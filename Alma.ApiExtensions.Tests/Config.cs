﻿using System;
using System.Diagnostics;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;

namespace Alma.ApiExtensions.Testes
{
    public static class Config
    {
        //TODO: Melhorar para mais de um projeto a ser testado com o mesmo host.
        public static Uri HostBase = new Uri("http://localhost:80/Temporary_Listen_Addresses/api" + Guid.NewGuid().ToString().Substring(0, 6));
        public static Uri IniciarSelfHost(
            TestContext testContext,
            Action<IAppBuilder, HttpConfiguration> startupConfig,
            Type owinFactory)
        {
            FinalizarTodosOsSelfHost();
            ConfigHttpHost.BuildAndDeployOwin(owinFactory, HostBase, startupConfig);
            Trace.WriteLine("OK");
            return HostBase;
        }

        public static void IniciarDatabase(
            TestContext testContext,
            string projetoBancoDeDados)
        {
            FinalizarDatabase();
            if (!string.IsNullOrWhiteSpace(projetoBancoDeDados))
                ConfigDatabase.BuildAndDeploy(testContext, projetoBancoDeDados);
        }

        public static void FinalizarTodosOsSelfHost()
        {
            Trace.Write("Limpando hosting... ");
            ConfigHttpHost.DropApplication();
            Trace.WriteLine("OK");
        }
        public static void FinalizarDatabase()
        {
            Trace.Write("Limpando banco de dados... ");
            ConfigDatabase.DropDatabase();
            Trace.WriteLine("OK");
        }

    }
}
