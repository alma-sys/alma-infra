﻿using Microsoft.AspNetCore.Authentication;
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
            _securityKey = new SymmetricSecurityKey(Alma.Common.Config.Settings.Jwt.GetKey());
            _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
            _jwtHeader = new JwtHeader(_signingCredentials);
        }

        public JsonWebToken Create(string login, string email, string given_name, string family_name, IList<string> roles, IList<Claim> additional_claims = null)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException(nameof(login));

            var expires = DateTime.UtcNow.AddMinutes(Alma.Common.Config.Settings.Jwt.ExpiryInMintures);

            var claims = new List<Claim>();
            if (!string.IsNullOrWhiteSpace(given_name))
                claims.Add(new Claim(JwtRegisteredClaimNames.GivenName, given_name));
            if (!string.IsNullOrWhiteSpace(family_name))
                claims.Add(new Claim(JwtRegisteredClaimNames.FamilyName, family_name));
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, login));
            claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, login));

            if (!string.IsNullOrWhiteSpace(email))
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, email));

            if (roles != null)
                foreach (var r in roles)
                    if (!string.IsNullOrWhiteSpace(r))
                        claims.Add(new Claim("role", r));

            if (additional_claims != null)
                foreach (var r in additional_claims)
                    claims.Add(r);

            var token = new JwtSecurityToken(
                issuer: Alma.Common.Config.Settings.Jwt.Issuer,
                audience: Alma.Common.Config.Settings.Jwt.Audiences[0],
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
                options.RequireHttpsMetadata = Alma.Common.Config.Settings.Https;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Alma.Common.Config.Settings.Jwt.Issuer,

                    ValidateAudience = true,
                    ValidAudiences = Alma.Common.Config.Settings.Jwt.Audiences,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Alma.Common.Config.Settings.Jwt.GetKey()),

                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role,
                };
                options.Validate();
            });

        }
    }
}
