using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TechTalk.SpecFlow;

namespace Alma.ApiExtensions.TestHelper.SpecFlow
{
    public static class Helpers
    {
        #region Entity Id
        public static Int64 EntityId(this ScenarioContext context)
        {
            return context.Get<Int64>(nameof(EntityId));
        }

        public static void EntityId(this ScenarioContext context, long value)
        {
            context.Set(value, nameof(EntityId));
        }

        public static T EntityId<T>(this FeatureContext context)
        {
            if (!context.Keys.Contains(nameof(EntityId)))
                return default(T);
            return context.Get<T>(nameof(EntityId));
        }

        public static void EntityId<T>(this FeatureContext context, T value)
        {
            context.Set(value, nameof(EntityId));
        }
        #endregion Entity Id

        #region Endpoint
        public static string Endpoint(this ScenarioContext context)
        {
            return context.Get<string>(nameof(Endpoint));
        }

        public static void Endpoint(this ScenarioContext context, string value)
        {
            context.Set(value, nameof(Endpoint));
        }

        #endregion Endpoint

        #region Argument List
        public static Table ArgumentList(this ScenarioContext context)
        {
            if (!context.Keys.Contains(nameof(ArgumentList)))
                return null;

            return context.Get<Table>(nameof(ArgumentList));
        }

        public static void ArgumentList(this ScenarioContext context, Table table)
        {
            context.Set(table, nameof(ArgumentList));
        }
        #endregion

        #region HttpMethod
        public static Method HttpMethod(this ScenarioContext context)
        {
            return context.Get<Method>(nameof(HttpMethod));
        }

        public static void HttpMethod(this ScenarioContext context, Method valor)
        {
            context.Set(valor, nameof(HttpMethod));
        }

        #endregion HttpMethod

        #region Argument
        public static Table Argument(this ScenarioContext context)
        {
            if (!context.Keys.Contains(nameof(Argument)))
                return null;
            return context.Get<Table>(nameof(Argument));
        }

        public static void Argument(this ScenarioContext context, Table table)
        {
            context.Set(table, nameof(Argument));
        }

        public static string JsonArgument(this ScenarioContext context)
        {
            if (!context.Keys.Contains(nameof(JsonArgument)))
                return null;
            return context.Get<string>(nameof(JsonArgument));
        }

        public static void JsonArgument(this ScenarioContext context, string value)
        {
            context.Set(value, nameof(JsonArgument));
        }

        public static string ModelType(this ScenarioContext context)
        {
            if (context.ContainsKey(nameof(ModelType)))
                return context.Get<string>(nameof(ModelType));

            return null;
        }

        public static void ModelType(this ScenarioContext context, string valor)
        {
            context.Set(valor, nameof(ModelType));
        }

        #endregion Argument

        #region Response
        public static IRestResponse Response(this ScenarioContext context)
        {
            return context.Get<object>(nameof(Response)) as IRestResponse;
        }

        public static IRestResponse<T> Response<T>(this ScenarioContext context)
        {
            return context.Get<object>(nameof(Response)) as IRestResponse<T>;
        }

        public static void Response(this ScenarioContext context, IRestResponse value)
        {
            context.Set((object)value, nameof(Response));
        }

        public static T ResponseData<T>(this ScenarioContext context) where T : new()
        {
            return ((IRestResponse<T>)context.Get<object>(nameof(Response))).Data;
        }
        #endregion Response

        #region Url Parameter

        public static Table UrlParameter(this ScenarioContext context)
        {
            if (!context.Keys.Contains(nameof(UrlParameter)))
                return null;
            return context.Get<Table>(nameof(UrlParameter));
        }

        public static void UrlParameter(this ScenarioContext context, Table table)
        {
            context.Set(table, nameof(UrlParameter));
        }

        #endregion

        #region Url

        public static string MontaUrl(this ScenarioContext context, string url)
        {
            var queryString = new List<string>();
            var parameters = context.UrlParameter();
            if (parameters == null || parameters.RowCount == 0)
                return url;

            foreach (var item in parameters.Rows)
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


        #region Access Object

        public static string AccessObject(this ScenarioContext context)
        {
            if (!context.Keys.Contains(nameof(AccessObject)))
                return null;
            return context.Get<string>(nameof(AccessObject));
        }

        public static void AccessObject(this ScenarioContext context, string value)
        {
            context.Set(value, nameof(AccessObject));
        }

        #endregion

    }

}
