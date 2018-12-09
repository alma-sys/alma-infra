using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace Alma.ApiExtensions.TestHelper.SpecFlow
{
    [Binding]
    sealed class Specs
    {

        private ScenarioContext ScenarioContext { get; set; }
        public Specs(ScenarioContext scenarioContext)
        {
            if (scenarioContext == null) throw new ArgumentNullException(nameof(scenarioContext));
            this.ScenarioContext = scenarioContext;
        }

        [BeforeScenario("ignore", "skip")]
        public void SkipTest()
        {
            ScenarioContext.Pending();
        }

        [Given(i18n.Messages_pt.HttpMethodIs, i18n.Messages_pt.Culture)]
        [Given(i18n.Messages.HttpMethodIs)]
        public void AndHttpMethodIs(string p0)
        {
            if (!Enum.TryParse(p0?.Trim().ToUpper(), out Method httpMethod))
            {
                Assert.True(false, "Http method not supported"); // from xunit docs to use Assert.Fail
            }

            ScenarioContext.HttpMethod(httpMethod);
        }

        [Given(i18n.Messages_pt.TheFollowingListOfArgumentsOfType, i18n.Messages_pt.Culture)]
        [Given(i18n.Messages.TheFollowingListOfArgumentsOfType)]
        public void GivenTheFollowingListOfArgumentsOfType(string typeName, Table table)
        {
            var tipo = SearchType(typeName);
            try
            {
                var reflValidarResposta = typeof(TableHelperExtensionMethods).GetMethod(nameof(TableHelperExtensionMethods.CreateSet), new[] { typeof(Table) }).GetGenericMethodDefinition();
                var genericMethod = reflValidarResposta.MakeGenericMethod(tipo);
                var arg = genericMethod.Invoke(null, new[] { table });

                var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                var json = JsonConvert.SerializeObject(arg, Formatting.Indented, jsonSerializerSettings);

                ScenarioContext.JsonArgument(json);
                ScenarioContext.ArgumentList(table);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }
        }

        [Given(i18n.Messages_pt.TheEndPointUrlIs, i18n.Messages_pt.Culture)]
        [Given(i18n.Messages.TheEndPointUrlIs)]
        public void GivenTheFollowingEndpointUrl(string p0)
        {
            ScenarioContext.Endpoint(p0);
        }

        [Given(i18n.Messages_pt.TheFollowingUrlParameters, i18n.Messages_pt.Culture)]
        [Given(i18n.Messages.TheFollowingUrlParameters)]
        public void GivenTheFollowingUrlParameters(Table table)
        {
            ScenarioContext.UrlParameter(table);
        }

        [Given(i18n.Messages_pt.ThatIInformedTheUrlParameterWithTheScenarioId, i18n.Messages_pt.Culture)]
        [Given(i18n.Messages.ThatIInformedTheUrlParameterWithTheScenarioId)]
        public void GivenThatIInformedTheUrlParameterWithTheScenarioId(string parameter)
        {
            var id = ScenarioContext.EntityId();
            AddUrlParameter(parameter, id.ToString());
        }

        [Given(i18n.Messages_pt.ThatIInformedTheUrlParameterWithValue, i18n.Messages_pt.Culture)]
        [Given(i18n.Messages.ThatIInformedTheUrlParameterWithValue)]
        public void GivenThatIInformedTheUrlParameterWithValue(string parameter, string value)
        {
            AddUrlParameter(parameter, value);
        }

        private void AddUrlParameter(string parameter, string value)
        {
            var table = ScenarioContext.UrlParameter();
            if (table == null)
                table = new Table("Field", "Value");

            table.AddRow(parameter, value);
            ScenarioContext.UrlParameter(table);
        }

        [Given(i18n.Messages_pt.TheFollowingValues, i18n.Messages_pt.Culture)]
        [Given(i18n.Messages.TheFollowingValues)]
        public void AndTheFollowingValues(string typeName, Table table)
        {
            var tipo = SearchType(typeName);

            try
            {
                var reflValidarResposta = typeof(TableHelperExtensionMethods).GetMethod(nameof(TableHelperExtensionMethods.CreateInstance), new[] { typeof(Table) }).GetGenericMethodDefinition();
                var genericMethod = reflValidarResposta.MakeGenericMethod(tipo);
                var arg = genericMethod.Invoke(null, new[] { table });

                var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                var json = JsonConvert.SerializeObject(arg, Formatting.Indented, jsonSerializerSettings);

                ScenarioContext.JsonArgument(json);
                ScenarioContext.ModelType(typeName);
                ScenarioContext.Argument(table);

            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }
        }

        [Then(i18n.Messages_pt.TheStatusCodeOfTheResponseShouldBe, i18n.Messages_pt.Culture)]
        [Then(i18n.Messages.TheStatusCodeOfTheResponseShouldBe)]
        public void ThenTheStatusCodeOfTheResponseShouldBe(string p0)
        {
            var response = ScenarioContext.Response();

            Assert.Equal(p0, response.StatusCode.ToString()); //xunit can't show a custom message
                                                              //response.StatusCode == HttpStatusCode.InternalServerError ?
                                                              //response.Content : null);
        }

        private Type SearchType(string nomeTipo)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x =>
                    !x.IsDynamic &&
                    !x.GetName().FullName.Contains("Microsoft.") &&
                    !x.GetName().FullName.Contains("System.")
                ).ToList();
            assemblies.AddRange(
                assemblies.AsParallel().SelectMany(a => a.GetReferencedAssemblies()
                    .Select(r => Assembly.Load(r)))
                );
            var tipos =
                assemblies.AsParallel()
                .SelectMany(x => x.GetTypes().Where(t => t.Name == nomeTipo).Select(t => t))
                .Distinct().ToArray();

            if (tipos.Length == 0)
                throw new ApplicationException(string.Format("Tipo {0} não encontrado.", nomeTipo));
            else if (tipos.Length > 1)
                throw new ApplicationException(string.Format("Tipo {0} encontrado em mais de um namespace.", nomeTipo));
            else
                return tipos.Single();
        }

        [When(i18n.Messages_pt.CallTheService, i18n.Messages_pt.Culture)]
        [When(i18n.Messages.CallTheService)]
        public void WhenCallTheService()
        {
            var url = (string)ScenarioContext.Endpoint();
            var endpoint = LocalServer.WebHostBaseUrl.ToString();
            if (!endpoint.EndsWith("/"))
                endpoint += "/";
            endpoint += url.Substring(url.StartsWith("/") ? 1 : 0);

            ExecuteRequest(endpoint);
        }

        [Then(i18n.Messages_pt.AResponseOfTypeXShouldBeReturnedWithTheFollowingValues, i18n.Messages_pt.Culture)]
        [Then(i18n.Messages.AResponseOfTypeXShouldBeReturnedWithTheFollowingValues)]
        public void ThenAResponseOfTypeXShouldBeReturnedWithTheFollowingValues(string modelType, Table table)
        {
            var modelTypeType = SearchType(modelType);
            ScenarioContext.ModelType(modelType);

            ValidateResponse(table, modelTypeType);
        }

        [Then(i18n.Messages_pt.AListOfTypeXShouldBeReturnedWithAnItemWithTheFollowingValues, i18n.Messages_pt.Culture)]
        [Then(i18n.Messages.AListOfTypeXShouldBeReturnedWithAnItemWithTheFollowingValues)]
        public void ThenAListOfTypeXShouldBeReturnedWithAnItemWithTheFollowingValues(string modelType, Table table)
        {
            var modelTypeType = SearchType(modelType);
            ScenarioContext.ModelType(modelType);
            ValidateResponseListWithOneItem(table, modelTypeType);
        }

        [Then(i18n.Messages_pt.AListOfTypeXShouldBeReturnedWithTheFollowingValues, i18n.Messages_pt.Culture)]
        [Then(i18n.Messages.AListOfTypeXShouldBeReturnedWithTheFollowingValues)]
        public void ThenAListOfTypeXShouldBeReturnedWithTheFollowingValues(string tipoModel, Table table)
        {
            var modelTypeType = SearchType(tipoModel);
            ScenarioContext.ModelType(tipoModel);
            ValidateResponseList(table, modelTypeType);
        }

        [Then(i18n.Messages_pt.ThePropertyXOfAllItemsShouldBe, i18n.Messages_pt.Culture)]
        [Then(i18n.Messages.ThePropertyXOfAllItemsShouldBe)]
        public void ThenThePropertyXOfAllItemsShouldBe(string property, string value)
        {
            var modelType = ScenarioContext.ModelType();
            if (string.IsNullOrWhiteSpace(modelType))
                throw new InvalidOperationException("Validate if a list was returned before checking for items.");
            var modelTypeType = SearchType(modelType);
            ValidateAResponseListHavingAPropertyWithTheSameValue(modelTypeType, property, value);
        }


        [Then(i18n.Messages_pt.AResponseWithAListOfTypeXShouldBeReturnedWithData, i18n.Messages_pt.Culture)]
        [Then(i18n.Messages.AResponseWithAListOfTypeXShouldBeReturnedWithData)]
        public void ThenAResponseWithAListOfTypeXShouldBeReturnedWithData(string modelType)
        {
            var modelTypeType = SearchType(modelType);
            ScenarioContext.ModelType(modelType);
            ValidateResponseListWithData(modelTypeType);
        }

        [Then(i18n.Messages_pt.APropertyXWithAListOfYWithTheFollowingValues, i18n.Messages_pt.Culture)]
        [Then(i18n.Messages.APropertyXWithAListOfYWithTheFollowingValues)]
        public void ThenAPropertyXWithAListOfYWithTheFollowingValues(string property, string listType, Table table)
        {
            try
            {

                var modelType = ScenarioContext.ModelType();
                var modelTypeType = SearchType(modelType);


                var response = ScenarioContext.Response();
                var content = response.Content;
                var returningObject = JsonConvert.DeserializeObject(content, modelTypeType);

                // Search for property
                var reflectionProp = returningObject.GetType().GetProperty(property, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                var propertyValue = reflectionProp.GetValue(returningObject);

                var listTypeType = SearchType(listType);
                //executar CompareToSet generica
                var reflectionValidateResponse = typeof(SetComparisonExtensionMethods).GetMethod(nameof(SetComparisonExtensionMethods.CompareToSet), System.Reflection.BindingFlags.Instance | BindingFlags.Static | System.Reflection.BindingFlags.Public).GetGenericMethodDefinition();
                var genericMethod = reflectionValidateResponse.MakeGenericMethod(listTypeType);
                genericMethod.Invoke(this, new[] { table, propertyValue });
                // The same of: table.CompareToSet<listTypeType>(propertyValue);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }
        }

        [Then(i18n.Messages_pt.APropertyXOfTypeYWithTheFollowingValues, i18n.Messages_pt.Culture)]
        [Then(i18n.Messages.APropertyXOfTypeYWithTheFollowingValues)]
        public void ThenAPropertyXOfTypeYWithTheFollowingValues(string property, string type, Table table)
        {
            try
            {
                var modelType = ScenarioContext.ModelType();
                var modelTypeType = SearchType(modelType);

                var response = ScenarioContext.Response();
                var content = response.Content;
                var returningObject = JsonConvert.DeserializeObject(content, modelTypeType);

                //executar Busca Propriedade
                var reflectionProperty = returningObject.GetType().GetProperty(property, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                var propertyValue = reflectionProperty.GetValue(returningObject);
                var propertyType = SearchType(type);

                //executar CompareToSet generica
                var reflectionValidate = typeof(InstanceComparisonExtensionMethods).GetMethod(nameof(InstanceComparisonExtensionMethods.CompareToInstance), System.Reflection.BindingFlags.Instance | BindingFlags.Static | System.Reflection.BindingFlags.Public).GetGenericMethodDefinition();
                var genericMethod = reflectionValidate.MakeGenericMethod(propertyType);
                genericMethod.Invoke(this, new[] { table, propertyValue });
                //The same of: table.CompareToSet<propertyType>(propertyValue);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }
        }


        [Given(i18n.Messages_pt.ThatIDontHaveAValidUserSession, i18n.Messages_pt.Culture)]
        [Given(i18n.Messages.ThatIDontHaveAValidUserSession)]
        public void GivenThatIDontHaveAValidUserSession()
        {
            ScenarioContext.AccessObject(null);
        }

        [Given(i18n.Messages_pt.ThatIHaveAValidUserSessionWithAnyPermission, i18n.Messages_pt.Culture)]
        [Given(i18n.Messages.ThatIHaveAValidUserSessionWithAnyPermission)]
        public void GivenThatIHaveAValidUserSessionWithAnyPermission()
        {
            GivenThatIHaveAValidUserSessionWithAccessToObject("dummy");
        }

        [Given(i18n.Messages_pt.ThatIHaveAValidUserSessionWithAccessToObject, i18n.Messages_pt.Culture)]
        [Given(i18n.Messages.ThatIHaveAValidUserSessionWithAccessToObject)]
        public void GivenThatIHaveAValidUserSessionWithAccessToObject(string objectName)
        {
            ScenarioContext.AccessObject(objectName);
        }

        /// <summary>
        /// Do the magic
        /// </summary>
        /// <param name="endpoint">EndPoint Url</param>
        /// <returns> IRestResponse object of Type Y </returns>
        private IRestResponse ExecuteRequest(string endpoint)
        {
            Trace.WriteLine("BDD: Creating request");
            var url = ScenarioContext.MontaUrl(endpoint);

            Trace.WriteLine($"BDD: Url => {url}");

            var request = new RestRequest
            {
                Method = ScenarioContext.HttpMethod()
            };
            Trace.WriteLine($"BDD: Method => {request.Method}");

            request.Parameters.Clear();

            if (request.Method == Method.POST)
            {
                var contentType = "application/json";
                var json = ScenarioContext.JsonArgument();
                if (!String.IsNullOrWhiteSpace(json))
                    request.AddParameter(contentType, json, ParameterType.RequestBody);

                Trace.WriteLine($"BDD: Content =>  {contentType}");
                Trace.WriteLine($"BDD: Body =>  {json}");
            }

            request.AddHeader("Accept", "application/json");
            AddAuthorization(request);

            var restClient = new RestClient(url);

            Trace.WriteLine("BDD: Executando request");
            var response = restClient.Execute(request);

            ScenarioContext.Response(response);
            Trace.WriteLine($"BDD: Response Status => {response.StatusCode}");
            Trace.WriteLine($"BDD: Response Content => {response.Content}");
            return response;
        }


        private string GetJsonArgument<T>()
        {
            object arg = null;
            if (ScenarioContext.ArgumentList() != null)
            {
                arg = ScenarioContext.ArgumentList().CreateSet<T>();
            }
            else if (ScenarioContext.Argument() != null)
            {
                arg = ScenarioContext.Argument().CreateInstance<T>();
            }

            if (arg == null)
                return string.Empty;

            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var json = JsonConvert.SerializeObject(arg, Formatting.Indented, jsonSerializerSettings);

            return json;
        }

        private void ValidateResponse(Table expectedTable, Type typeOfActualModel)
        {
            var response = ScenarioContext.Response();
            var content = response.Content;

            var actual = JsonConvert.DeserializeObject(content, typeOfActualModel);

            expectedTable.CompareToInstance(actual);
        }

        private void ValidateResponseList(Table expectedTable, Type typeOfActualModel)
        {
            try
            {
                var response = ScenarioContext.Response();
                var content = response.Content;

                var type = typeof(List<>).MakeGenericType(typeOfActualModel);
                var actual = JsonConvert.DeserializeObject(content, type) as IList;

                var reflValidarResposta = typeof(SetComparisonExtensionMethods).GetMethod(nameof(SetComparisonExtensionMethods.CompareToSet), System.Reflection.BindingFlags.Instance | BindingFlags.Static | System.Reflection.BindingFlags.Public).GetGenericMethodDefinition();
                var genericMethod = reflValidarResposta.MakeGenericMethod(typeOfActualModel);
                genericMethod.Invoke(this, new object[] { expectedTable, actual });
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }

        }

        private void ValidateResponseListWithData(Type typeOfActualModel)
        {
            var response = ScenarioContext.Response();
            var content = response.Content;

            var type = typeof(List<>).MakeGenericType(typeOfActualModel);

            var model = JsonConvert.DeserializeObject(content, type) as IList;

            Assert.NotNull(model);
            Assert.NotEmpty(model);
            Assert.All(model.Cast<object>(), x =>
            {
                Assert.NotNull(x);
                Assert.IsType(typeOfActualModel, x);
            });
        }

        private void ValidateResponseListWithOneItem(Table expectedTable, Type typeOfActualModel)
        {
            var response = ScenarioContext.Response();
            var content = response.Content;

            var type = typeof(List<>).MakeGenericType(typeOfActualModel);
            var model = JsonConvert.DeserializeObject(content, type) as IList;

            Assert.NotNull(model);
            Assert.Equal(1, model.Count);
            Assert.All(model.Cast<object>(), x =>
            {
                Assert.NotNull(x);
                Assert.IsType(typeOfActualModel, x);
            });

            expectedTable.CompareToInstance(model[0]);
        }
        private void ValidateAResponseListHavingAPropertyWithTheSameValue(Type itemType, string property, string value)
        {
            var response = ScenarioContext.Response();
            var content = response.Content;

            var type = typeof(List<>).MakeGenericType(itemType);
            var model = JsonConvert.DeserializeObject(content, type) as IList;

            Assert.NotNull(model);
            Assert.NotEmpty(model);
            Assert.All(model.Cast<object>(), x =>
            {
                Assert.NotNull(x);
                Assert.IsType(itemType, x);
            });

            var reflectionProperty = type.GetProperty(property, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (reflectionProperty == null)
                throw new InvalidOperationException(string.Format("Could not find property {0} in the object {1}", property, type.Name));
            foreach (var item in model)
            {
                //Search for property
                var propertyValue = reflectionProperty.GetValue(item);

                if (value == null)
                    Assert.Null(propertyValue);
                else
                    Assert.Equal(value, propertyValue.ToString());
            }
        }



        private void AddAuthorization(RestRequest request)
        {
            var objeto = ScenarioContext.AccessObject();
            if (!string.IsNullOrWhiteSpace(objeto))
            {
                if (AuthorizationTokenFunc == null)
                    throw new InvalidOperationException($"Please set the {nameof(AuthorizationTokenFunc)}.");
                var token = AuthorizationTokenFunc(ScenarioContext, new string[] { objeto });
                if (!string.IsNullOrWhiteSpace(token))
                    request.AddHeader("Authorization", "Bearer " + token);
            }
        }

        /// <summary>
        /// Defina a função para obter token de autorização
        /// </summary>
        public static Func<ScenarioContext, string[], string> AuthorizationTokenFunc { get; set; }

    }
}
