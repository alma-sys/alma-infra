using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.DotNet.PlatformAbstractions;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Alma.ApiExtensions.Log
{
    static class HtmlLogger
    {
        public static StringBuilder CreateHtmlFromException(Exception exception, HttpContext httpContext)
        {
            var sb = new StringBuilder();
            var currentException = exception;

            var htmlError = string.Empty;
            {
                var app = "AspNetCore";
                try
                {
                    app = Assembly.GetEntryAssembly().GetName().Name;
                }
                catch { }


                Func<Exception, int, string> recursiveException = null;

                recursiveException = new Func<Exception, int, string>((e, i) =>
                {
                    var r = new StringBuilder();
                    if (e != null)
                    {
                        var pad = "".PadLeft(i * 3, ' ');
                        r.AppendLine($"{pad}{e.GetType().FullName}: {e.Message.Replace(Environment.NewLine, Environment.NewLine + pad)}");
                        r.AppendLine($"{pad}HelpLink: {e.HelpLink}");
                        r.AppendLine($"{pad}Source: {e.Source}");
                        r.AppendLine($"{pad}------");
                        if (e is AggregateException)
                        {
                            foreach (var ie in ((AggregateException)e).InnerExceptions)
                                r.Append(recursiveException(ie, ++i));
                        }
                        else
                            r.Append(recursiveException(e.InnerException, ++i));
                    }
                    return r.ToString();
                });


                htmlError = string.Format(@"<html>
<head>        <style>
         body {{font-family:""Verdana"";font-weight:normal;font-size: .7em;color:black;}} 
         p {{font-family:""Verdana"";font-weight:normal;color:black;margin-top: -5px}}
         b {{font-family:""Verdana"";font-weight:bold;color:black;margin-top: -5px}}
         H1 {{ font-family:""Verdana"";font-weight:normal;font-size:18pt;color:red }}
         H2 {{ font-family:""Verdana"";font-weight:normal;font-size:14pt;color:maroon }}
         pre {{font-family:""Lucida Console"";font-size: .9em}}
         .marker {{font-weight: bold; color: black;text-decoration: none;}}
         .version {{color: gray;}}
         .error {{margin-bottom: 10px;}}
         .expandable {{ text-decoration:underline; font-weight:bold; color:navy; cursor:hand; }}
        </style></head>
<body>
    <h1>Server Error in '{0}' Application.<hr width=""100%"" size=""1"" color=""silver""></h1>
    <h2>{1}</h2>    
<p><b>Exception Details: </b><code><pre style=""background-color: #ffffcc; padding: 8px; font-size: 1.1em"">
{2}
</pre></code></p>
<p><b>Stack Trace: </b><code><pre style=""background-color: #ffffcc; padding: 8px; font-size: 1.1em"">
{3}
</pre></code></p>
</body></html>", app, currentException.Message, recursiveException(currentException, 0), currentException.StackTrace);
            }

            var bodyEnd = htmlError.IndexOf("</body>");
            if (bodyEnd == -1)
            {
                sb.Append(exception.ToString());
            }
            else
            {
                sb.Append(htmlError.Substring(0, bodyEnd));

                try
                {
                    foreach (var key in exception.Data.Keys)
                    {
                        sb.AppendFormat("<br /><b>{0}:</b><br />{1}<br />\r\n", key, exception.Data[key]);
                    }
                    if (exception.InnerException != null)
                        foreach (var key in exception.InnerException.Data.Keys)
                        {
                            sb.AppendFormat("<br /><b>{0}:</b><br />{1}<br />\r\n", key, exception.InnerException.Data[key]);
                        }
                }
                catch { }

                GenerateHttpContextDetails(sb, httpContext);

                try
                {
                    AggregateException ex_multiple =
                        exception as AggregateException ??
                        exception.InnerException as AggregateException;
                    if (ex_multiple != null && ex_multiple.InnerExceptions != null && ex_multiple.InnerExceptions.Count > 0)
                    {
                        sb.AppendFormat("<br /><b>Multiple Exceptions:</b><br />\r\n");

                        sb.AppendFormat("<ul>\r\n");
                        foreach (var ex in ex_multiple.InnerExceptions)
                        {
                            sb.AppendFormat("<li>{0} - {1}<br /><code><pre style=\"background-color: #ffffcc; padding: 8px; font-size: 1.1em\">{2}</pre></code></li>\r\n",
                                ex.GetType().FullName,
                                ex.Message,
                                ex.ToString()
                                );
                        }
                        sb.AppendFormat("</ul>\r\n");
                    }
                }
                catch { }

                sb.AppendLine("</body></html>");
            }

            return sb;
        }

        private static void GenerateHttpContextDetails(StringBuilder sb, HttpContext context)
        {
            if (context == null)
                return;

            try
            {
                sb.AppendFormat("<br /><b>Url:</b><br />{0}<br />\r\n", context.Request.GetUri());
            }
            catch { }
            try
            {
                sb.AppendFormat("<br /><b>User Agent:</b><br />{0}<br />\r\n", context.Request.Headers["User-Agent"]);
            }
            catch { }
            try
            {
                sb.AppendFormat("<br /><b>User Host Address:</b><br />{0}<br />\r\n", context.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress);
            }
            catch { }
            try
            {
                sb.AppendFormat("<br /><b>Server Machine Name:</b><br />{0}<br />\r\n", Environment.MachineName);
            }
            catch { }
            try
            {
                sb.AppendFormat("<br /><b>Server Base Path:</b><br />{0}<br />\r\n", ApplicationEnvironment.ApplicationBasePath);
            }
            catch { }

            try
            {
                sb.AppendFormat("<br /><b>Url Referrer:</b><br />{0}<br />\r\n", context.Request.Headers["Referer"]);
            }
            catch { }

            try
            {
                if (!context.User.Identity.IsAuthenticated)
                    sb.AppendFormat("<br /><b>Logged User:</b><br />None<br />\r\n");
                else
                    sb.AppendFormat("<br /><b>Logged User:</b><br />{0}<br />\r\n", context.User.Identity.Name);
            }
            catch { }

            try
            {
                sb.AppendFormat("<br /><b>Request Headers:</b><br />\r\n");
                sb.AppendFormat("<ul style=\"background-color: #ffffcc\">\r\n");
                foreach (var h in context.Request.Headers.Keys)
                {
                    sb.AppendFormat("<li>{0}: {1}</li>\r\n", h, context.Request.Headers[h]);

                }
                sb.AppendFormat("</ul>\r\n");
            }
            catch { }


            try
            {
                if (context.Request.Method == "POST")
                {
                    sb.AppendFormat("<br /><b>Post Data:</b><br />\r\n");
                    if (context.Request.Form.Keys.Count > 0)
                    {
                        sb.AppendFormat("<ul style=\"background-color: #ffffcc\">\r\n");
                        foreach (string formKey in context.Request.Form.Keys)
                        {
                            if (!formKey.ToLower().Contains("senha") && !formKey.ToLower().Contains("password"))
                                sb.AppendFormat("<li>{0}: {1}</li>\r\n", formKey, context.Request.Form[formKey]);
                        }
                        sb.AppendFormat("</ul>\r\n");
                    }
                    else
                    {
                        //json, xml and other types of docs
                        sb.Append("<code><pre style=\"background-color: #ffffcc; padding: 8px; font-size: 1.1em\">");
                        try
                        {
                            context.Request.Body.Seek(0, SeekOrigin.Begin);
                        }
                        catch { }
                        sb.Append(System.Web.HttpUtility.HtmlEncode(new StreamReader(context.Request.Body).ReadToEnd()));
                        sb.Append("</pre></code><br />");

                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendFormat("<br /><b>Error Getting Post Data:</b><br />{0}<br />\r\n", ex.Message);
            }
        }

        //private static string FindExceptionName(Exception exception)
        //{
        //    string filename = exception.GetType().Name;
        //    if (filename.ToLower().Contains("http") && exception.InnerException != null)
        //        return FindExceptionName(exception.InnerException);
        //    else
        //        return filename;
        //}
    }


}
