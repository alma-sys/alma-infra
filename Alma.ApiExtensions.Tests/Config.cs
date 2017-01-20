using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;

namespace Alma.ApiExtensions.Tests
{
    public static class Config
    {
        private static int HostPorta = 80; // new Random().Next(10000, 10003);
        public static Uri HostBase = new Uri("http://localhost:" + HostPorta.ToString() + "/Temporary_Listen_Addresses/api" + Guid.NewGuid().ToString().Substring(0, 6));//TODO: Melhorar pra mais de um projeto

        public static void IniciarSelfHost(
            TestContext testContext,
            Action<IAppBuilder, HttpConfiguration> startupConfig,
            Type owinFactory,
            string projetoBancoDeDados = null)
        {
            FinalizarHost();
            //var task1 = Task.Factory.StartNew(() => ConfigHttpHost.BuildAndDeploy(testContext, ProjetosWeb));
            //var task2 = Task.Factory.StartNew(() => ConfigDatabase.BuildAndDeploy(testContext, ProjetoBancoDeDados));
            //Task.WaitAll(task1, task2);

            if (!string.IsNullOrWhiteSpace(projetoBancoDeDados))
                ConfigDatabase.BuildAndDeploy(testContext, projetoBancoDeDados);

            ConfigHttpHost.BuildAndDeployOwin(owinFactory, HostBase, startupConfig);

            Trace.WriteLine("OK");
        }

        public static void FinalizarHost()
        {
            Trace.Write("Limpando banco de dados e iis express... ");
            var task1 = Task.Factory.StartNew(() => ConfigDatabase.DropDatabase());
            var task2 = Task.Factory.StartNew(() => ConfigHttpHost.DropApplication());
            Task.WaitAll(task1, task2);
            Trace.WriteLine("OK");
        }


    }
}
