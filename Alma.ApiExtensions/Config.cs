using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;

namespace Alma.ApiExtensions
{
    public static class Config
    {
        public const string cfgLogErros = Core.Config.cfgRoot + "logerros";
        public const string cfgSistemaRemetente = Core.Config.cfgRoot + "sistema-remetente";
        public static string cfgUploads => Core.Config.cfgRoot.Replace(":", "-") + "uploads";

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

        private static void ConfigurarLogExceptions()
        {
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Erros");

#if !DEBUG
            System.Web.HttpApplication.RegisterModule(typeof(Log.LogModule));
#endif
        }
    }
}
