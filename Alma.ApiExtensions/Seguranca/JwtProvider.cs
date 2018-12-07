using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Alma.ApiExtensions.Security
{
    class JwtProvider : IJwtProvider
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        private readonly SecurityKey _securityKey;
        private readonly SigningCredentials _signingCredentials;
        private readonly JwtHeader _jwtHeader;


        public JwtProvider()
        {
            _securityKey = new SymmetricSecurityKey(Config.JwtKey);
            _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
            _jwtHeader = new JwtHeader(_signingCredentials);
        }

        public JsonWebToken Create(string login, string email, string given_name, string family_name, IList<string> roles, IList<Claim> additional_claims = null)
        {

            var expires = DateTime.UtcNow.AddMinutes(Config.JwtExpiryInMintures);

            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.GivenName, given_name));
            claims.Add(new Claim(JwtRegisteredClaimNames.FamilyName, family_name));
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, login));
            claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, login));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, email));

            if (roles != null)
                foreach (var r in roles)
                    claims.Add(new Claim("role", r));

            if (additional_claims != null)
                foreach (var r in additional_claims)
                    claims.Add(r);

            var token = new JwtSecurityToken(
                issuer: Config.JwtIssuer,
                audience: Config.JwtAudiences[0],
                claims: claims,
                expires: expires,
                signingCredentials: _signingCredentials);


            var token_str = _jwtSecurityTokenHandler.WriteToken(token);

            return new JsonWebToken
            (
                accessToken: token_str,
                expires: expires
            );
        }
    }

    public class JsonWebToken
    {
        public JsonWebToken(string accessToken, DateTime expires)
        {
            this.AccessToken = accessToken;
            this.Expires = expires;
        }

        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; private set; }
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty(PropertyName = "expires_at")]
        public DateTime Expires { get; private set; }
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType => JwtBearerDefaults.AuthenticationScheme;
    }

    public interface IJwtProvider
    {
        JsonWebToken Create(string login, string email, string given_name, string family_name, IList<string> roles, IList<Claim> additional_claims = null);
    }


    public static class JwtServicesExtensions
    {
        public static AuthenticationBuilder AddJwtAuthentication(this IServiceCollection services)
        {
            return services.AddAuthentication(options =>
            {
                options.DefaultScheme =
                options.DefaultAuthenticateScheme =
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = Config.Https;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Config.JwtIssuer,

                    ValidateAudience = true,
                    ValidAudiences = Config.JwtAudiences,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Config.JwtKey),

                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role,
                };
                options.Validate();
            });

        }
    }
}
