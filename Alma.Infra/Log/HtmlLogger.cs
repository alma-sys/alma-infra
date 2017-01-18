using System;
using System.IO;
using System.Text;
using System.Web;

namespace Alma.Infra.Log
{
    static class HtmlLogger
    {
        public static StringBuilder Montar(Exception exception, HttpContext context)
        {
            var sb = new StringBuilder();
            var currentException = exception as HttpException;
            if (currentException == null)
                currentException = new HttpUnhandledException(exception.Message, exception);

            string htmlError = currentException.GetHtmlErrorMessage();
            if (string.IsNullOrEmpty(htmlError))
            {
                var app = "Unknown";
                try
                {
                    app = VirtualPathUtility.ToAppRelative("~");
                }
                catch { }

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
</body></html>", app, currentException.Message, currentException, currentException.StackTrace);
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

                if (context != null)
                {
                    try
                    {
                        sb.AppendFormat("<br /><b>Url:</b><br />{0}<br />\r\n", context.Request.RawUrl);
                    }
                    catch { }
                    try
                    {
                        sb.AppendFormat("<br /><b>User Agent:</b><br />{0}<br />\r\n", context.Request.UserAgent);
                    }
                    catch { }
                    try
                    {
                        sb.AppendFormat("<br /><b>User Host Address:</b><br />{0}<br />\r\n", context.Request.UserHostAddress);
                    }
                    catch { }
                    try
                    {
                        sb.AppendFormat("<br /><b>Server Machine Name:</b><br />{0}<br />\r\n", context.Server.MachineName);
                    }
                    catch { }

                    try
                    {
                        sb.AppendFormat("<br /><b>Url Referrer:</b><br />{0}<br />\r\n", context.Request.UrlReferrer);
                    }
                    catch { }

                    try
                    {
                        if (!context.Request.IsAuthenticated)
                            sb.AppendFormat("<br /><b>Logged Windows User:</b><br />None<br />\r\n");
                        else
                            sb.AppendFormat("<br /><b>Logged Windows User:</b><br />{0}<br />\r\n", context.Request.LogonUserIdentity.Name);
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
                        foreach (var h in context.Request.Headers.AllKeys)
                        {
                            sb.AppendFormat("<li>{0}: {1}</li>\r\n", h, context.Request.Headers[h]);

                        }
                        sb.AppendFormat("</ul>\r\n");
                    }
                    catch { }


                    try
                    {
                        if (context.Request.HttpMethod == "POST")
                        {
                            sb.AppendFormat("<br /><b>Post Data:</b><br />\r\n");
                            if (context.Request.Form.Keys.Count > 0)
                            {
                                sb.AppendFormat("<ul style=\"background-color: #ffffcc\">\r\n");
                                foreach (string formKey in HttpContext.Current.Request.Form.Keys)
                                {
                                    if (!formKey.ToLower().Contains("senha") && !formKey.ToLower().Contains("password"))
                                        sb.AppendFormat("<li>{0}: {1}</li>\r\n", formKey, HttpContext.Current.Request.Form[formKey]);
                                }
                                sb.AppendFormat("</ul>\r\n");
                            }
                            else
                            {
                                //json, xml e outros docs 
                                sb.Append("<code><pre style=\"background-color: #ffffcc; padding: 8px; font-size: 1.1em\">");
                                try
                                {
                                    context.Request.InputStream.Seek(0, SeekOrigin.Begin);
                                }
                                catch { }
                                sb.Append(HttpUtility.HtmlEncode(new StreamReader(context.Request.InputStream).ReadToEnd()));
                                sb.Append("</pre></code><br />");

                            }
                        }
                    }
                    catch { }
                }

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
