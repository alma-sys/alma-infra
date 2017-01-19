using System;
using System.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace Alma.ApiExtensions.Log
{
    public class LogModule : IHttpModule
    {
        public void Dispose()
        {
        }

        private static bool _startWasCalled;

        public void Init(HttpApplication app)
        {
            if (_startWasCalled) return;
            _startWasCalled = true;

            CaminhoErro = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Erros");
            CaminhoLog = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Logs");
            var config = ConfigurationManager.GetSection("system.web/customErrors") as System.Web.Configuration.CustomErrorsSection;
            if (config != null && config.Mode != System.Web.Configuration.CustomErrorsMode.Off)
                ErrosPersonalizados = true;

            app.Error += App_Error;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            System.Web.Http.GlobalConfiguration.Configuration.Services.Add(typeof(System.Web.Http.ExceptionHandling.IExceptionLogger), new WebApiExceptionLogger());
        }

        private class WebApiExceptionLogger : System.Web.Http.ExceptionHandling.ExceptionLogger
        {
            public override void Log(ExceptionLoggerContext context)
            {
                LogModule module = null;
                foreach (var key in HttpContext.Current.ApplicationInstance.Modules.AllKeys)
                {
                    var mod = HttpContext.Current.ApplicationInstance.Modules[key];
                    if (mod is LogModule)
                    {
                        module = (LogModule)mod;
                        break;
                    }
                }
                //deixa dar exception se for null?
                module.OnError(context.Exception);
                base.Log(context);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            OnError((Exception)e.ExceptionObject);
        }

        public static string CaminhoLog { get; private set; }
        public static string CaminhoErro { get; private set; }
        public static bool ErrosPersonalizados { get; private set; }

        private void App_Error(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            if (context == null)
                throw new InvalidOperationException();
            var ex = context.Server.GetLastError();
            OnError(ex);
        }

        protected virtual void OnError(Exception exception)
        {

            var httpException = exception as HttpException;

            if (httpException != null && httpException.GetHttpCode() < 500)
            {
                //filtro de 404
                //não logar ou configurar?
            }
            else
            {
                var context = HttpContext.Current;

                var isAjax = context.Request != null && context.Request.Headers.Get("X-Requested-With") == "XMLHttpRequest";
                var appType = context.ApplicationInstance.GetType();
                if (appType.Assembly.GetName().Name.Contains(".asax"))
                    appType = appType.BaseType;

                var content = HtmlLogger.Montar(exception, context).ToString();

                EnviarEmail(content, false, appType.Assembly.GetName().Name);
                SalvarArquivo(content, false, appType.Assembly.GetName().Name);
                //CriarTicketGitLab();
            }

        }


        public static DateTime emailUltimaData = DateTime.MinValue;
        public static int emailEnviados = 0;
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

                using (var file = System.IO.File.Create(fileName))
                using (var stream = new System.IO.StreamWriter(file))
                {
                    stream.Write(text);
                    stream.Flush();
                }
            }
            catch { }

        }

        private static void CriarTicketGitLab()
        {
            //throw new NotImplementedException();
        }

    }
}
