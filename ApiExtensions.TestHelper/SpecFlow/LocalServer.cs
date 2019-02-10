using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using TechTalk.SpecFlow;

namespace Alma.ApiExtensions.TestHelper.SpecFlow
{
    [Binding]
    public class LocalServer
    {
        private static IWebHost webHost;

        public static Uri WebHostBaseUrl { get; private set; }

        [BeforeTestRun()]
        void StartLocalServer()
        {
            if (webHost == null)
                Trace.TraceWarning($"You didn't start a web server for specflow tests. If you need to host api's, please call {nameof(LocalServer)}.{nameof(StartLocalServer)}");
        }

        [AfterTestRun()]
        void DisposeLocalServer()
        {
            if (webHost != null)
                webHost.Dispose();
        }

        /// <summary>
        /// Start a local server to use in SpecFlow tests.
        /// </summary>
        /// <param name="baseHostUrl">Base URL to listen for requests.</param>
        /// <param name="app">The delegate that configures <see cref="IApplicationBuilder" />.</param>
        public static void StartLocalServer(Uri baseHostUrl, Action<IApplicationBuilder> app)
        {
            if (baseHostUrl == null)
                throw new ArgumentNullException(nameof(baseHostUrl));
            WebHostBaseUrl = baseHostUrl;
            webHost = WebHost.StartWith(baseHostUrl.ToString(), app);
            webHost.StartAsync();

        }


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
    }
}
