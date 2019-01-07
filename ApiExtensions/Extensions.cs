using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ApiExtensions.Tests")]

namespace Alma.ApiExtensions
{
    internal static class Extensions
    {

        public static byte[] GetKey(this Common.JwtSetting setting)
        {
            try
            {
                return Base64UrlEncoder.DecodeBytes(setting.Base64Key);
            }
            catch (Exception ex)
            {
                throw new System.Configuration.ConfigurationErrorsException($"Invalid configuration at {nameof(Alma.Common.Config.Settings.Jwt)}:{nameof(Alma.Common.Config.Settings.Jwt.Base64Key)}", ex);
            }

        }

        public static Uri GetUri(this HttpRequest request)
        {
            var builder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Value,
                Path = request.Path,
                Query = request.QueryString.ToUriComponent()
            };
            return builder.Uri;
        }
    }
}
