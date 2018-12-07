using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.DotNet.PlatformAbstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Alma.ApiExtensions.Log
{
    public static class LogModuleExtensions
    {
        public static IApplicationBuilder UseLogModule(this IApplicationBuilder builder)
        {
            LogModule.CaminhoErro = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Erros");
            LogModule.CaminhoLog = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Logs");

            if (!Directory.Exists(LogModule.CaminhoErro))
                Directory.CreateDirectory(LogModule.CaminhoErro);
            if (!Directory.Exists(LogModule.CaminhoLog))
                Directory.CreateDirectory(LogModule.CaminhoLog);


            return builder.UseMiddleware<LogModule>();
        }

        public static void Report(this Exception ex, HttpContext context)
        {
            LogModule.OnError(context, ex);
        }

        public static void Report(this Exception ex)
        {
            HttpContext ctx = null;
            try
            {
                ctx = new HttpContextAccessor().HttpContext;
            }
            catch { }

            LogModule.OnError(ctx, ex);
        }

    }
    public class LogModule : IDisposable
    {

        private readonly RequestDelegate _next;

        public LogModule(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Do something with context near the beginning of request processing.



            //var config = ConfigurationManager.GetSection("system.web/customErrors") as System.Web.Configuration.CustomErrorsSection;
            //if (config != null && config.Mode != System.Web.Configuration.CustomErrorsMode.Off)
            //    ErrosPersonalizados = true;
            ErrosPersonalizados = true;

            //app.Error += App_Error;
            //AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


            try
            {

                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                OnError(context, ex);
                throw;
            }

        }





        public void Dispose()
        {
        }


        //private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    OnError((Exception)e.ExceptionObject);
        //}

        public static string CaminhoLog { get; internal set; }
        public static string CaminhoErro { get; internal set; }
        public static bool ErrosPersonalizados { get; private set; }


        internal static void OnError(HttpContext context, Exception exception)
        {

            var httpException = (Exception)null; // exception as HttpException;

            if (httpException != null) // && httpException.GetHttpCode() < 500)
            {
                //filtro de 404
                //não logar ou configurar?
            }
            else
            {

                //var isAjax = context.Request != null && context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                //var appType = context.ApplicationInstance.GetType();
                //if (appType.Assembly.GetName().Name.Contains(".asax"))
                //    appType = appType.BaseType;
                var appType = Path.GetFileNameWithoutExtension(ApplicationEnvironment.ApplicationBasePath);
                if (string.IsNullOrWhiteSpace(appType))
                    appType = System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Name;

                var content = HtmlLogger.Montar(exception, context).ToString();

                EnviarEmail(content, false, appType);
                SalvarArquivo(content, false, appType);
                //CriarIncidente();
            }

        }


        private static DateTime emailUltimaData = DateTime.MinValue;
        private static int emailEnviados = 0;
        private static void EnviarEmail(string conteudo, bool erroTratado, string tag)
        {
            if (emailUltimaData != DateTime.Today)
            {
                emailUltimaData = DateTime.Today;
                emailEnviados = 0;
            }
            emailEnviados++;
            var totalEmails = ApiExtensions.Config.EmailsErroPorDia;

            if (emailEnviados <= totalEmails)
            {
                new System.Threading.Thread(delegate ()
                {
                    try
                    {
                        var mail = new MailMessage();
                        foreach (var email in ApiExtensions.Config.DestinosEmailErro)
                            mail.To.Add(email);
                        mail.IsBodyHtml = conteudo.Contains("<body");
                        mail.Body = conteudo;
                        mail.Subject = string.Format("{0} - {1} | {2} de {3} emails por dia", tag, erroTratado ? "Log" : "Exception", emailEnviados, totalEmails);

                        var smtp = new SmtpClient();
                        smtp.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        SalvarArquivo(ex.ToString(), true, tag + ".email");
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }).Start();
            }
        }

        private static void SalvarArquivo(string text, bool erroTratado, string tag)
        {
            if (string.IsNullOrEmpty(tag))
                tag = "erro";


            var fileName = DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss-fff_") + tag + ".html";
            if (erroTratado)
                fileName = System.IO.Path.Combine(CaminhoLog, fileName);
            else
                fileName = System.IO.Path.Combine(CaminhoErro, fileName);

            try
            {
                if (!Directory.Exists(CaminhoLog))
                    Directory.CreateDirectory(CaminhoLog);
                if (!Directory.Exists(CaminhoErro))
                    Directory.CreateDirectory(CaminhoErro);


                Trace.WriteLine(fileName, "log");
                using (var file = System.IO.File.Create(fileName))
                using (var stream = new System.IO.StreamWriter(file))
                {
                    stream.Write(text);
                    stream.Flush();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }

        }

        private static void CriarIncidente()
        {
            //throw new NotImplementedException();
        }

    }
}
