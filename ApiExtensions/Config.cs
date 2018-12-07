using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ApiExtensions.Tests")]

namespace Alma.ApiExtensions
{
    public static class Config
    {
        public const string cfgLogErros = Common.Config.cfgRoot + "logerros";
        public const string cfgSistemaRemetente = Common.Config.cfgRoot + "sistema-remetente";
        public static string cfgUploads => Common.Config.cfgRoot.Replace(":", "-") + "uploads";

        public static IList<MailAddress> DestinosEmailErro
        {
            get
            {
                try
                {
                    var emails = ConfigurationManager.AppSettings[cfgLogErros];
                    var enderecos = emails.Split(';').Select(x => new MailAddress(x)).ToArray();

                    return enderecos;
                }
                catch (Exception ex)
                {
                    throw new ConfigurationErrorsException(
                        $"Configuração faltando ou inválida em {cfgLogErros} na App.Config ou Web.Config.", ex);
                }
            }
        }

        public static MailAddress EmailSistemaRemetente
        {
            get
            {
                try
                {
                    var emails = ConfigurationManager.AppSettings[cfgSistemaRemetente];
                    var enderecos = emails.Split(';').Select(x => new MailAddress(x)).ToArray();

                    return enderecos.FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static int EmailsErroPorDia
        {
            get
            {
                return 65;
            }
        }


        internal static byte[] JwtKey
        {
            get
            {
                var str = ConfigurationManager.AppSettings[Common.Config.cfgRoot + "jwtkey"];
                try
                {
                    return Base64UrlEncoder.DecodeBytes(str);
                }
                catch (Exception ex)
                {
                    throw new ConfigurationErrorsException("Invalid configuration at " + Common.Config.cfgRoot + "jwtkey", ex);
                }
            }
        }

        internal static string JwtIssuer
        {
            get
            {
                var str = ConfigurationManager.AppSettings[Common.Config.cfgRoot + "jwtissuer"];
                if (string.IsNullOrWhiteSpace(str))
                    throw new ConfigurationErrorsException("Invalid configuration at " + Common.Config.cfgRoot + "jwtissuer");
                return str;
            }
        }
        internal static int JwtExpiryInMintures => Convert.ToInt32(ConfigurationManager.AppSettings[Common.Config.cfgRoot + "jwtexpiry"] ?? "5");
        //internal static int JwtRefreshTokenExpiryInMintures => Convert.ToInt32(ConfigurationManager.AppSettings[Core.Config.cfgRoot + "jwtrefreshtokenexpiry"]);
        internal static string[] JwtAudiences => ConfigurationManager.AppSettings[Common.Config.cfgRoot + "jwtaudiences"]?.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();
        internal static bool Https => Convert.ToBoolean(ConfigurationManager.AppSettings[Common.Config.cfgRoot + "https"] ?? "true");
    }

    internal static class Extensions
    {
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
