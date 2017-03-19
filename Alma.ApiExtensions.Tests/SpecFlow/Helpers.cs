using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestSharp;
using TechTalk.SpecFlow;

namespace Alma.ApiExtensions.Testes.SpecFlow
{
    public static class Helpers
    {
        #region IdEntidade
        public static Int64 IdEntidade(this ScenarioContext context)
        {
            return context.Get<Int64>("idEntidade");
        }

        public static void IdEntidade(this ScenarioContext context, Int64 valor)
        {
            context.Set(valor, "idEntidade");
        }

        public static T IdEntidade<T>(this FeatureContext context)
        {
            if (!context.Keys.Contains("idEntidade"))
                return default(T);
            return context.Get<T>("idEntidade");
        }

        public static void IdEntidade<T>(this FeatureContext context, T valor)
        {
            context.Set(valor, "idEntidade");
        }
        #endregion IdEntidade

        #region Endpoint
        public static String Endpoint(this ScenarioContext context)
        {
            return context.Get<String>("endpoint");
        }

        public static void Endpoint(this ScenarioContext context, String valor)
        {
            context.Set(valor, "endpoint");
        }

        #endregion Endpoint

        #region Lista Argumentos
        public static Table ListaArgumentos(this ScenarioContext context)
        {
            if (!context.Keys.Contains("listaArgumentos"))
                return null;

            return context.Get<Table>("listaArgumentos");
        }

        public static void ListaArgumentos(this ScenarioContext context, Table valor)
        {
            context.Set(valor, "listaArgumentos");
        }
        #endregion

        #region HttpMethod
        public static Method HttpMethod(this ScenarioContext context)
        {
            return context.Get<Method>("httpMethod");
        }

        public static void HttpMethod(this ScenarioContext context, Method valor)
        {
            context.Set(valor, "httpMethod");
        }

        #endregion HttpMethod

        #region Argumento
        public static Table Argumento(this ScenarioContext context)
        {
            if (!context.Keys.Contains("argumento"))
                return null;
            return context.Get<Table>("argumento");
        }

        public static void Argumento(this ScenarioContext context, Table valor)
        {
            context.Set(valor, "argumento");
        }

        public static String JsonArgumento(this ScenarioContext context)
        {
            if (!context.Keys.Contains("jsonArgumento"))
                return null;
            return context.Get<String>("jsonArgumento");
        }

        public static void JsonArgumento(this ScenarioContext context, String valor)
        {
            context.Set(valor, "jsonArgumento");
        }

        public static string TipoModel(this ScenarioContext context)
        {
            if (context.ContainsKey("tipoModel"))
                return context.Get<string>("tipoModel");

            return null;
        }

        public static void TipoModel(this ScenarioContext context, string valor)
        {
            context.Set(valor, "tipoModel");
        }

        #endregion Argumento

        #region Response
        public static IRestResponse Response(this ScenarioContext context)
        {
            return context.Get<object>("response") as IRestResponse;
        }

        public static IRestResponse<T> Response<T>(this ScenarioContext context)
        {
            return context.Get<object>("response") as IRestResponse<T>;
        }

        public static void Response(this ScenarioContext context, IRestResponse valor)
        {
            context.Set((object)valor, "response");
        }

        public static T ResponseData<T>(this ScenarioContext context) where T : new()
        {
            return ((IRestResponse<T>)context.Get<object>("response")).Data;
        }
        #endregion Response

        #region ParametroUrl

        public static Table ParametroUrl(this ScenarioContext context)
        {
            if (!context.Keys.Contains("parametroUrl"))
                return null;
            return context.Get<Table>("parametroUrl");
        }

        public static void ParametroUrl(this ScenarioContext context, Table valor)
        {
            context.Set(valor, "parametroUrl");
        }

        #endregion

        #region Url

        public static string MontaUrl(this ScenarioContext context, string url)
        {
            List<String> queryString = new List<String>();
            var parametros = context.ParametroUrl();
            if (parametros == null || parametros.RowCount == 0)
                return url;

            foreach (var item in parametros.Rows)
            {
                var placeholder = "{" + string.Format("{0}", item["Field"]) + "}";
                if (url.IndexOf(placeholder) >= 0)
                    url = url.Replace(placeholder, item["Value"]);
                else
                    queryString.Add(String.Format("{0}={1}", item["Field"], WebUtility.UrlEncode(item["Value"])));
            }

            if (queryString.Count > 0) url += "?" + String.Join("&", queryString);

            return url;
        }

        #endregion


        #region Perfil Acesso

        public static string ObjetoAcesso(this ScenarioContext context)
        {
            if (!context.Keys.Contains("objetoAcesso"))
                return null;
            return context.Get<string>("objetoAcesso");
        }

        public static void ObjetoAcesso(this ScenarioContext context, string valor)
        {
            context.Set(valor, "objetoAcesso");
        }

        #endregion

    }

}
