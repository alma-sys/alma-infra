using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Xml;

namespace Alma.Dados.Api
{
    public abstract class ApiBase
    {
        protected abstract string UrlBase { get; }
        protected abstract string Proxy { get; }

        protected virtual string AcceptLanguage
        {
            get
            {
                return System.Globalization.CultureInfo.CurrentCulture.Name;
            }
        }

        private CookieCollection lastCookies;
        protected virtual WebRequest CreateRequest(string rota)
        {
            string url;
            if (rota.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || rota.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                url = rota;
            }
            else
            {
                if (!rota.StartsWith("/") && !rota.StartsWith("?"))
                    rota = "/" + rota;
                url = UrlBase + rota;
            }
            var request = (HttpWebRequest)WebRequest.Create(url);
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.CachePolicy = noCachePolicy;

            if (!string.IsNullOrEmpty(Proxy))
                request.Proxy = new WebProxy(Proxy);

            if (lastCookies != null && lastCookies.Count > 0)
            {
                var container = new CookieContainer();
                foreach (Cookie c in lastCookies)
                {
                    container.Add(c);
                }
                request.CookieContainer = container;
            }

            if (!string.IsNullOrWhiteSpace(AcceptLanguage))
                request.Headers.Add(HttpRequestHeader.AcceptLanguage, AcceptLanguage);

            return request;
        }

        protected abstract string LogRawFileName(string rota);

        private void LogRaw(string rota, string raw)
        {
            try
            {
                var path = LogRawFileName(rota);
                if (string.IsNullOrWhiteSpace(path))
                    return;
                using (var stream = new StreamWriter(path))
                {
                    stream.Write(raw);
                    stream.Close();
                }

            }
            catch
            { }
        }

        protected virtual string GetUrlRaw(string rota, object post = null)
        {
            var request = (HttpWebRequest)CreateRequest(rota);

            try
            {
                if (post != null)
                {
                    if (post is string && ((string)post).Contains("xmlns"))
                    {
                        var bytes = System.Text.Encoding.UTF8.GetBytes((string)post);
                        request.Method = "POST";
                        request.ContentType = "text/xml";
                        request.ContentLength = bytes.Length;
                        using (var postStream = request.GetRequestStream())
                        {
                            postStream.Write(bytes, 0, bytes.Length);
                            postStream.Flush();
                        }
                    }
                    else
                    {
                        var postValuesDict = post is IDictionary<string, object> ?
                            new System.Web.Routing.RouteValueDictionary((IDictionary<string, object>)post) :
                            new System.Web.Routing.RouteValueDictionary((object)post);

                        var postValues =
                            string.Join("&", (from key in postValuesDict.Keys
                                              select key + "=" + System.Web.HttpUtility.UrlEncode(Convert.ToString(postValuesDict[key]))));

                        var bytes = System.Text.Encoding.UTF8.GetBytes(postValues);
                        request.Method = "POST";
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.ContentLength = bytes.Length;
                        using (var postStream = request.GetRequestStream())
                        {
                            postStream.Write(bytes, 0, bytes.Length);
                            postStream.Flush();
                        }
                    }
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    FixCookies(request, response);
                    //salvar cookies para a proxima requisição
                    if (response.Cookies != null && response.Cookies.Count > 0)
                    {
                        if (this.lastCookies != null)
                            foreach (Cookie c in response.Cookies)
                            {
                                var existente = lastCookies.OfType<Cookie>().Where(x => x.Name == c.Name).FirstOrDefault();
                                if (existente == null)
                                    lastCookies.Add(c);
                                else
                                    existente.Value = c.Value;
                            }
                        else
                            this.lastCookies = response.Cookies;
                    }

                    var reader = new System.IO.StreamReader(response.GetResponseStream());

                    var raw = reader.ReadToEnd();

                    if (!raw.Contains("{") && !raw.Contains("[") && !raw.Contains("<"))
                        throw new WebException(raw, null, WebExceptionStatus.UnknownError, response);

                    LogRaw(rota, raw);
                    return raw;

                }
            }
            catch (WebException ex)
            {
                if (request.Proxy is WebProxy proxy)
                {
                    ex.Data.Add("ext-api_proxy", proxy.Address);
                }

                ex.Data.Add("ext-api_rota", rota);
                ex.Data.Add("ext-api_url", request.RequestUri.ToString());
                try
                {
                    var stream = ex.Response.GetResponseStream();
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    var message = new System.IO.StreamReader(stream).ReadToEnd();
                    if (message.Contains("<body"))
                    {
                        message = new Regex("<[^>]*>").Replace(message, "");
                    }
                    ex.Data.Add("ext-api_error", message);

                }
                catch { }
                try
                {
                    if (request.Credentials as NetworkCredential != null)
                    {
                        ex.Data.Add("userName", (request.Credentials as NetworkCredential).UserName);
                    }
                    if (request.Credentials as CredentialCache != null)
                    {
                        foreach (var c in (request.Credentials as CredentialCache))
                        {
                            if (c as NetworkCredential != null)
                            {
                                ex.Data.Add("userName", (c as NetworkCredential).UserName);
                            }
                        }
                    }
                    if (request.Headers.AllKeys.Contains("token"))
                    {
                        ex.Data.Add("token", request.Headers["token"]);
                    }

                }
                catch { }
                throw;
            }
        }

        private static void FixCookies(HttpWebRequest request, HttpWebResponse response)
        {
            for (int i = 0; i < response.Headers.Count; i++)
            {
                string name = response.Headers.GetKey(i);
                if (name != "Set-Cookie")
                    continue;
                string value = response.Headers.Get(i);
                foreach (var singleCookie in value.Split(','))
                {
                    Match match = Regex.Match(singleCookie, "(.+?)=(.+?);");
                    if (match.Captures.Count == 0)
                        continue;
                    response.Cookies.Add(
                        new Cookie(
                            match.Groups[1].ToString(),
                            match.Groups[2].ToString(),
                            "/",
                            request.Host.Split(':')[0]));
                }
            }
        }

        protected virtual Dictionary<string, object> GetUrlJson(string rota, object post = null)
        {
            var json = GetUrlRaw(rota, post);
            var serializer = new JavaScriptSerializer();
            if (json.StartsWith("["))
            {
                var obj = serializer.Deserialize<Dictionary<string, object>[]>(json);
                var dict = new Dictionary<string, object>
                {
                    { "", obj }
                };
                return dict;
            }
            else
            {
                var obj = serializer.Deserialize<Dictionary<string, object>>(json);
                return obj;
            }
        }

        protected virtual XmlDocument GetUrlXml(string rota, object post = null)
        {
            var xml = GetUrlRaw(rota, post);

            var settings = new XmlReaderSettings()
            {
                XmlResolver = null,
                DtdProcessing = DtdProcessing.Ignore
            };
            var stream = new StringReader(xml);
            var reader = XmlReader.Create(stream, settings);


            var doc = new XmlDocument();
            doc.Load(reader);

            return doc;
        }
    }
}
