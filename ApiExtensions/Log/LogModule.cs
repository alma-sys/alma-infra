using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.DotNet.PlatformAbstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Alma.ApiExtensions.Log
{
    public static class LogModuleExtensions
    {
        public static IApplicationBuilder UseLogModule(this IApplicationBuilder builder)
        {
            // TODO: Be able to store into a file/blob storage
            LogModule.ErrorFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Erros");
            LogModule.LogFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Logs");

            if (!Directory.Exists(LogModule.ErrorFilePath))
                Directory.CreateDirectory(LogModule.ErrorFilePath);
            if (!Directory.Exists(LogModule.LogFilePath))
                Directory.CreateDirectory(LogModule.LogFilePath);


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
            //    CustomErrors = true;
            CustomErrors = true;

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

        public static string LogFilePath { get; internal set; }
        public static string ErrorFilePath { get; internal set; }
        public static bool CustomErrors { get; private set; }


        internal static void OnError(HttpContext context, Exception exception)
        {

            var httpException = (Exception)null; // exception as HttpException;

            if (httpException != null) // && httpException.GetHttpCode() < 500)
            {
                //404 filter
                // TODO: Ignore or configure?
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

                var content = HtmlLogger.CreateHtmlFromException(exception, context).ToString();

                SendEmail(content, false, appType);
                SaveFile(content, false, appType);
            }

        }


        private static DateTime lastEmailDate = DateTime.MinValue;
        private static int totalEmailSent = 0;
        private static void SendEmail(string content, bool handled, string tag)
        {
            if (lastEmailDate != DateTime.Today)
            {
                lastEmailDate = DateTime.Today;
                totalEmailSent = 0;
            }
            totalEmailSent++;
            var totalEmails = Alma.Common.Config.Settings.Logging.MaxEmailsPerDay;

            if (totalEmailSent <= totalEmails)
            {
                Task.Run(() =>
                {

                    try
                    {
                        var mail = new MailMessage();
                        foreach (var email in Alma.Common.Config.Settings.Logging.MailDestinations)
                            mail.To.Add(email);
                        mail.IsBodyHtml = content.Contains("<body");
                        mail.Body = content;
                        mail.Subject = string.Format("{0} - {1} | {2} de {3} emails por dia", tag, handled ? "Log" : "Exception", totalEmailSent, totalEmails);

                        var smtp = new SmtpClient();

#if NETSTANDARD
                        smtp.Host = Alma.Common.Config.Settings.Smtp.Host;
                        smtp.Port = Alma.Common.Config.Settings.Smtp.Port;
                        smtp.EnableSsl = Alma.Common.Config.Settings.Smtp.Ssl;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(Alma.Common.Config.Settings.Smtp.UserName, Alma.Common.Config.Settings.Smtp.Password);
#endif

                        smtp.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        SaveFile(ex.ToString(), true, tag + ".email");
                        System.Diagnostics.Trace.TraceError(ex.ToString());
                    }
                });
            }
        }

        private static void SaveFile(string text, bool handled, string tag)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            if (string.IsNullOrEmpty(tag))
                tag = "erro";


            var fileName = DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss-fff_") + tag;
            if (text.Contains("<html"))
                fileName += ".html";
            else
                fileName += ".log";

            if (handled)
                fileName = System.IO.Path.Combine(LogFilePath, fileName);
            else
                fileName = System.IO.Path.Combine(ErrorFilePath, fileName);

            try
            {
                if (!Directory.Exists(LogFilePath))
                    Directory.CreateDirectory(LogFilePath);
                if (!Directory.Exists(ErrorFilePath))
                    Directory.CreateDirectory(ErrorFilePath);


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


    }
}
