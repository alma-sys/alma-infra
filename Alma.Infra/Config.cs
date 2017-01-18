using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Autofac;
//using FluentValidation;
[assembly: PreApplicationStartMethod(typeof(Alma.Infra.Config), "Iniciar")]

namespace Alma.Infra
{


    public static class Config
    {




        public static IList<MailAddress> DestinosEmailErro
        {
            get
            {
                try
                {
                    var emails = ConfigurationManager.AppSettings["alma:logerros"];
                    var enderecos = emails.Split(';').Select(x => new MailAddress(x)).ToArray();

                    return enderecos;
                }
                catch (Exception ex)
                {
                    throw new System.Configuration.ConfigurationErrorsException(
                        "Configuração faltando ou inválida em alma:logerros na App.Config ou Web.Config.", ex);
                }
            }
        }

        public static MailAddress EmailSistemaRemetente
        {
            get
            {
                try
                {
                    var emails = ConfigurationManager.AppSettings["alma:sistema-remetente"];
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




        public static void ConfigurarIoC(ContainerBuilder builder)
        {
            builder.RegisterModule<Alma.Infra.DependencyResolver.RepositoriosModule>();
            builder.RegisterModule<Alma.Infra.DependencyResolver.ValidadoresModule>();

            //registrar
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
