using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Alma.ApiExtensions.Security
{
    public static class ApiKeyAuthenticationExtensions
    {
        public static AuthenticationBuilder AddApiKey<TAuthService>(this AuthenticationBuilder builder, string realm)
        where TAuthService : class, IApiKeyAuthenticationService
        {
            return AddApiKey<TAuthService>(builder, ApiKeyAuthenticationHandler.SchemeName, (x) => x.Realm = realm);
        }


        private static AuthenticationBuilder AddApiKey<TAuthService>(this AuthenticationBuilder builder, Action<ApiKeyAuthenticationOptions> configureOptions)
            where TAuthService : class, IApiKeyAuthenticationService
        {
            return AddApiKey<TAuthService>(builder, ApiKeyAuthenticationHandler.SchemeName, configureOptions);
        }

        private static AuthenticationBuilder AddApiKey<TAuthService>(this AuthenticationBuilder builder, string authenticationScheme, Action<ApiKeyAuthenticationOptions> configureOptions)
            where TAuthService : class, IApiKeyAuthenticationService
        {
            builder.Services.AddSingleton<IPostConfigureOptions<ApiKeyAuthenticationOptions>, ApiKeyAuthenticationPostConfigureOptions>();
            builder.Services.AddTransient<IApiKeyAuthenticationService, TAuthService>();

            return builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                authenticationScheme, configureOptions);
        }

    }

    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Realm { get; set; }
    }

    public interface IApiKeyAuthenticationService
    {
        /// <summary>
        /// Returns the username for a valid key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Username when the key is valid. Null otherwise.</returns>
        Task<IEnumerable<Claim>> GetClaimsForKeyAsync(string key);
    }

    class ApiKeyAuthenticationPostConfigureOptions : IPostConfigureOptions<ApiKeyAuthenticationOptions>
    {
        public void PostConfigure(string name, ApiKeyAuthenticationOptions options)
        {
            if (string.IsNullOrEmpty(options.Realm))
            {
                throw new InvalidOperationException("Realm must be provided in options");
            }
        }
    }

    class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string AuthorizationHeaderName = "Authorization";
        internal const string SchemeName = "ApiKey";
        private readonly IApiKeyAuthenticationService _authenticationService;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IApiKeyAuthenticationService authenticationService)
            : base(options, logger, encoder, clock)
        {
            _authenticationService = authenticationService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                //Authorization header not in request
                return AuthenticateResult.NoResult();
            }

            if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out AuthenticationHeaderValue headerValue))
            {
                //Invalid Authorization header
                return AuthenticateResult.NoResult();
            }

            if (!SchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                //Not Basic authentication header
                return AuthenticateResult.NoResult();
            }


            var validUser = await _authenticationService.GetClaimsForKeyAsync(headerValue.Parameter);

            if (validUser == null)
            {
                return AuthenticateResult.Fail("Invalid key");
            }
            var claims = new List<Claim>(validUser);
            claims.Add(new Claim(ClaimTypes.Role, Options.Realm));

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"{SchemeName} realm=\"{Options.Realm}\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }
    }
}
