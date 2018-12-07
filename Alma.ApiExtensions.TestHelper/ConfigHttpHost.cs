using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;

namespace Alma.ApiExtensions.TestHelper
{
    static class ConfigHttpHost
    {

        private class IISInfo
        {
            public IDisposable Instance { get; internal set; }
            public Uri Uri { get; internal set; }
            public string ProjetoWeb { get; internal set; }
            public string TestConfig { get; internal set; }
            public string WebConfig { get; internal set; }
            public string WebConfigBackedUp { get; internal set; }
        }

        private static IList<IISInfo> iis = new List<IISInfo>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory">Usually Microsoft.Owin.Host.HttpListener.OwinHttpListener</param>
        /// <param name="hostBase"></param>
        /// <param name="setConfig"></param>
        internal static void BuildAndDeployOwin(Type factory, Uri hostBase, Action<IAppBuilder> setConfig)
        {
            Trace.Write("Levantando http host... ");
            if (hostBase == null)
                throw new ArgumentNullException(nameof(hostBase));

            if (setConfig == null)
                throw new ArgumentNullException(nameof(hostBase));

            var iisInfo = new IISInfo();

            var options = new StartOptions(hostBase.ToString())
            {
                ServerFactory = factory.Namespace
            };

            var urlLocal = hostBase.ToString().Replace("localhost", "+");
            var urlReal = hostBase.ToString().Replace("localhost", GetFQDN());

            var server = WebApp.Start(urlLocal, (app) =>
            {
                var listener = (HttpListener)app.Properties["System.Net.HttpListener"];
                listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

                //var config = new HttpConfiguration();
                //new HttpServer(config);

                setConfig(app); //, config);
            });
            Trace.WriteLine("Http host iniciado em " + urlReal);

            //var wait = new AutoResetEvent(false);
            //wait.WaitOne(); //aguardar para testes com browser se precisar
            iisInfo.Uri = new Uri(urlReal);



            iisInfo.Instance = server;
            iis.Add(iisInfo);
        }

        //internal static void BuildAndDeployWebApi(Uri hostBase, Action<HttpConfiguration> setConfig)
        //{
        //    if (hostBase == null)
        //        throw new ArgumentNullException(nameof(hostBase));

        //    if (setConfig == null)
        //        throw new ArgumentNullException(nameof(hostBase));
        //    var iisInfo = new IISInfo();
        //    iisInfo.Uri = hostBase;

        //    var config = new System.Web.Http.SelfHost.HttpSelfHostConfiguration(iisInfo.Uri);

        //    setConfig(config);
        //    var server = new System.Web.Http.SelfHost.HttpSelfHostServer(config);
        //    server.OpenAsync().Wait();
        //    iisInfo.Instance = server;

        //    iis.Add(iisInfo);
        //}

        private static string GetFQDN()
        {
            string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = Dns.GetHostName();

            domainName = "." + domainName;
            if (!hostName.EndsWith(domainName))  // if hostname does not already include domain name
            {
                hostName += domainName;   // add the domain name part
            }

            return hostName;                    // return the fully qualified name
        }

        internal static void DropApplication()
        {
            foreach (var iisInfo in iis)
            {
                iisInfo.Instance.Dispose();
            }
            iis.Clear();
        }
    }
}
