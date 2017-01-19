using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Alma.ApiExtensions.Tests.SpecFlow
{
    [Binding]
    public partial class GenericSteps
    {

        [Given(@"o método http é '(.*)'")]
        public void EOMetodoHttpE(string p0)
        {
            var metodo = Method.POST;

            switch (p0.ToUpper())
            {
                case "POST":
                    metodo = Method.POST;
                    break;
                case "GET":
                    metodo = Method.GET;
                    break;
                case "PUT":
                    metodo = Method.PUT;
                    break;
                case "DELETE":
                    metodo = Method.DELETE;
                    break;
                default:
                    Assert.Fail("Método HTTP não esperado");
                    break;
            }

            ScenarioContext.Current.HttpMethod(metodo);
        }

        [Given(@"a seguinte lista de argumentos do tipo '(.*)':")]
        public void DadoASeguinteListaDeArgumentos(string nomeTipo, Table table)
        {
            var tipo = LocalizarTipo(nomeTipo);

            var reflValidarResposta = typeof(TableHelperExtensionMethods).GetMethod("CreateSet", new[] { typeof(Table) }).GetGenericMethodDefinition();
            var genericMethod = reflValidarResposta.MakeGenericMethod(tipo);
            var arg = genericMethod.Invoke(null, new[] { table });

            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var json = JsonConvert.SerializeObject(arg, Formatting.Indented, jsonSerializerSettings);

            ScenarioContext.Current.JsonArgumento(json);
            ScenarioContext.Current.ListaArgumentos(table);
        }

        [Given(@"que a url do endpoint é '(.*)'")]
        public void DadoQueAUrlDoEndpointE(string p0)
        {
            ScenarioContext.Current.Endpoint(p0);
        }

        [Given(@"os seguintes parâmetros de url:")]
        public void DadoOsSeguintesParametrosDeUrl(Table table)
        {
            ScenarioContext.Current.ParametroUrl(table);
        }

        [Given(@"informei o parâmetro de url '(.*)' com o id do cenário")]
        public void DadoInformeiOParametroDeUrlComOIdDoCenario(string parametro)
        {
            Int64 id = ScenarioContext.Current.IdEntidade();
            AdicionaParametroUrl(parametro, id.ToString());
        }

        [Given(@"informei o parâmetro de url '(.*)' com o valor '(.*)'")]
        public void DadoInformeiOParametroDeUrlComOValor(string parametro, string valor)
        {
            AdicionaParametroUrl(parametro, valor);
        }

        private void AdicionaParametroUrl(String parametro, String valor)
        {
            var table = ScenarioContext.Current.ParametroUrl();
            if (table == null)
                table = new Table("Field", "Value");

            table.AddRow(parametro, valor);
            ScenarioContext.Current.ParametroUrl(table);
        }

        [Given(@"o seguinte argumento do tipo '(.*)':")]
        public void EOsSeguintesValores(string nomeTipo, Table table)
        {
            var tipo = LocalizarTipo(nomeTipo);

            var reflValidarResposta = typeof(TableHelperExtensionMethods).GetMethod("CreateInstance", new[] { typeof(Table) }).GetGenericMethodDefinition();
            var genericMethod = reflValidarResposta.MakeGenericMethod(tipo);
            var arg = genericMethod.Invoke(null, new[] { table });

            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var json = JsonConvert.SerializeObject(arg, Formatting.Indented, jsonSerializerSettings);

            ScenarioContext.Current.JsonArgumento(json);
            ScenarioContext.Current.TipoModel(nomeTipo);
            ScenarioContext.Current.Argumento(table);
        }

        [Then(@"statuscode da resposta deverá ser '(.*)'")]
        public void EntaoStatuscodeDaRespostaDeveraSer(string p0)
        {
            var response = ScenarioContext.Current.Response();

            Assert.AreEqual(p0, response.StatusCode.ToString(),
                response.StatusCode == HttpStatusCode.InternalServerError ?
                response.Content : null);
        }

        private static Type LocalizarTipo(string nomeTipo)
        {
            var assemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            var tipos = (from assemblyName in assemblies
                         from type in Assembly.Load(assemblyName).GetTypes()
                         where type.Name == nomeTipo
                         select type).ToArray();

            if (tipos.Length == 0)
                throw new ApplicationException(string.Format("Tipo {0} não encontrado.", nomeTipo));
            else if (tipos.Length > 1)
                throw new ApplicationException(string.Format("Tipo {0} encontrado em mais de um namespace.", nomeTipo));
            else
                return tipos.Single();
        }

        [When(@"chamar o servico")]
        public void QuandoChamarOServico()
        {
            var url = (String)ScenarioContext.Current.Endpoint();
            var endpoint = Config.HostBase.ToString();
            if (!endpoint.EndsWith("/"))
                endpoint += "/";
            endpoint += url.Substring(url.StartsWith("/") ? 1 : 0);

            ExecutarRequest(endpoint);
        }

        [Then(@"uma resposta do tipo '(.*)' deve ser retornada com os seguintes valores:")]
        public void EntaoUmaRespostaDoTipoXSerRetornadaComOsSeguintesValores(string tipoModel, Table table)
        {
            var tipoModelType = LocalizarTipo(tipoModel);
            ScenarioContext.Current.TipoModel(tipoModel);
            try
            {
                //executar ValidarResposta generica
                var reflValidarResposta = typeof(GenericSteps).GetMethod(nameof(ValidarResposta), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetGenericMethodDefinition();
                var genericMethod = reflValidarResposta.MakeGenericMethod(tipoModelType);
                genericMethod.Invoke(null, new[] { table });
                //equivalenta a: GenericSteps.ValidarResposta<tipoModelType>(table);

            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }
        }

        [Then(@"uma lista do tipo '(.*)' deve ser retornada com um item com os seguintes valores:")]
        public void EntaoUmaRespostaDoTipoXSerRetornadaComUmItemComOsSeguintesValores(string tipoModel, Table table)
        {
            var tipoModelType = LocalizarTipo(tipoModel);
            ScenarioContext.Current.TipoModel(tipoModel);
            try
            {
                //executar ValidarResposta generica
                var reflValidarResposta = typeof(GenericSteps).GetMethod(nameof(ValidarRespostaListaComUmItem), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetGenericMethodDefinition();
                var genericMethod = reflValidarResposta.MakeGenericMethod(tipoModelType);
                genericMethod.Invoke(null, new[] { table });
                //equivalenta a: GenericSteps.ValidarRespostaListaComUmItem<tipoModelType>(table);

            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }

        }

        [Then(@"a propriedade '(.*)' de todos os itens deve ser '(.*)'")]
        public void EntaoAPropriedadeDeTodosOsItensDeveSer(string propriedade, string valor)
        {
            var tipoModel = ScenarioContext.Current.TipoModel();
            if (string.IsNullOrWhiteSpace(tipoModel))
                throw new InvalidOperationException("Validar se a lista foi retornada com dados antes de validar propriedades.");
            var tipoModelType = LocalizarTipo(tipoModel);

            try
            {
                //executar ValidarResposta generica
                var reflValidarResposta = typeof(GenericSteps).GetMethod(nameof(ValidarRespostaListaComTodosOsItensComPropriedade), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetGenericMethodDefinition();
                var genericMethod = reflValidarResposta.MakeGenericMethod(tipoModelType);
                genericMethod.Invoke(null, new[] { propriedade, valor });
                //equivalenta a: GenericSteps.ValidarRespostaListaComTodosOsItensComPropriedade<tipoModelType>(propriedade, valor);

            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }
        }


        [Then(@"uma resposta com uma lista do tipo '(.*)' deve ser retornada com dados")]
        public void EntaoUmaRespostaComUmaListaDoTipoXSerRetornadaComItens(string tipoModel)
        {
            var tipoModelType = LocalizarTipo(tipoModel);
            ScenarioContext.Current.TipoModel(tipoModel);
            try
            {
                //executar ValidarResposta generica
                var reflValidarResposta = typeof(GenericSteps).GetMethod(nameof(ValidarRespostaListaComDados), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetGenericMethodDefinition();
                var genericMethod = reflValidarResposta.MakeGenericMethod(tipoModelType);
                genericMethod.Invoke(null, new object[] { });
                //equivalenta a: GenericSteps.ValidarRespostaListaComDados<tipoModelType>();

            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }

        }

        [Then(@"uma propriedade '(.*)' com uma lista de '(.*)' com os seguintes valores:")]
        public void EntaoUmaPropriedadeXComUmaListaDeY(string propriedade, string tipoLista, Table table)
        {
            try
            {

                var tipoModel = ScenarioContext.Current.TipoModel();
                var tipoModelType = LocalizarTipo(tipoModel);


                var response = ScenarioContext.Current.Response();
                var content = response.Content;
                var objetoRetorno = JsonConvert.DeserializeObject(content, tipoModelType);

                //executar Busca Propriedade
                var reflPropriedade = objetoRetorno.GetType().GetProperty(propriedade, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                var propriedadeObj = reflPropriedade.GetValue(objetoRetorno);
                //equivalenta a: table.CompareToSet<tipoListaType>(propriedadeObj);


                var tipoListaType = LocalizarTipo(tipoLista);


                //executar CompareToSet generica
                var reflValidarResposta = typeof(SetComparisonExtensionMethods).GetMethod("CompareToSet", System.Reflection.BindingFlags.Instance | BindingFlags.Static | System.Reflection.BindingFlags.Public).GetGenericMethodDefinition();
                var genericMethod = reflValidarResposta.MakeGenericMethod(tipoListaType);
                genericMethod.Invoke(null, new[] { table, propriedadeObj });
                //equivalenta a: table.CompareToSet<tipoListaType>(propriedadeObj);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }
        }

        [Given(@"que não tenho uma sessão de usuário ativa")]
        public void DadoQueNaoTenhoUmaSessaoDeUsuarioAtiva()
        {
            ScenarioContext.Current.Condominio(null);
            ScenarioContext.Current.ObjetoAcesso(null);
        }

        [Given(@"que tenho uma sessão de usuário ativa no condomínio '(.*)' com alguma permissão no sistema")]
        public void DadoQueTenhoUmaSessaoDeUsuarioAtivaNoCondominioComAlgumaPermissaoNoSistema(string condominio)
        {
            DadoOPerfilDeAcessoECondominioEContemOObjeto(condominio, "objeto");
        }

        [Given(@"que tenho uma sessão de usuário ativa no condomínio '(.*)' com permissão ao objeto '(.*)'")]
        public void DadoOPerfilDeAcessoECondominioEContemOObjeto(string condominio, string objeto)
        {
            ScenarioContext.Current.Condominio(condominio);
            ScenarioContext.Current.ObjetoAcesso(objeto);
        }

        /// <summary>
        /// Executa a mágica
        /// </summary>
        /// <param name="endpoint">Url da requisição</param>
        /// <returns> Objeto IRestResponse do Tipo Y </returns>
        public static IRestResponse ExecutarRequest(String endpoint)
        {
            var url = ScenarioContext.Current.MontaUrl(endpoint);

            var request = new RestRequest();

            request.Method = ScenarioContext.Current.HttpMethod();

            request.Parameters.Clear();

            if (request.Method == Method.POST)
            {
                var json = ScenarioContext.Current.JsonArgumento();
                if (!String.IsNullOrWhiteSpace(json))
                    request.AddParameter("application/json", json, ParameterType.RequestBody);
            }

            request.AddHeader("Accept", "application/json");
            AdicionarAutorizacao(request);

            var restClient = new RestClient(url);

            var response = restClient.Execute(request);

            ScenarioContext.Current.Response(response);

            return response;
        }


        private static String GetJsonArgumento<T>()
        {
            object arg = null;
            if (ScenarioContext.Current.ListaArgumentos() != null)
            {
                arg = ScenarioContext.Current.ListaArgumentos().CreateSet<T>();
            }
            else if (ScenarioContext.Current.Argumento() != null)
            {
                arg = ScenarioContext.Current.Argumento().CreateInstance<T>();
            }

            if (arg == null)
                return String.Empty;

            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var json = JsonConvert.SerializeObject(arg, Formatting.Indented, jsonSerializerSettings);

            return json;
        }

        public static void ValidarResposta<T>(Table table) where T : new()
        {
            var response = ScenarioContext.Current.Response();
            var content = response.Content;

            var model = JsonConvert.DeserializeObject<T>(content);

            table.CompareToInstance(model);
        }

        public static void ValidarRespostaLista<T>(Table table) where T : new()
        {
            var response = ScenarioContext.Current.Response();
            var content = response.Content;

            var model = JsonConvert.DeserializeObject<List<T>>(content);

            table.CompareToSet<T>(model);
        }

        public static void ValidarRespostaListaComDados<T>() where T : new()
        {
            var response = ScenarioContext.Current.Response();
            var content = response.Content;

            var model = JsonConvert.DeserializeObject<List<T>>(content);

            CollectionAssert.AllItemsAreNotNull(model);
            CollectionAssert.AllItemsAreInstancesOfType(model, typeof(T));
            Assert.AreNotEqual(0, model.Count);
        }

        public static void ValidarRespostaListaComUmItem<T>(Table table) where T : new()
        {
            var response = ScenarioContext.Current.Response();
            var content = response.Content;

            var model = JsonConvert.DeserializeObject<List<T>>(content);

            CollectionAssert.AllItemsAreNotNull(model);
            CollectionAssert.AllItemsAreInstancesOfType(model, typeof(T));
            Assert.AreEqual(1, model.Count);

            table.CompareToInstance(model.First());
        }
        public static void ValidarRespostaListaComTodosOsItensComPropriedade<T>(string propriedade, string valor) where T : new()
        {
            var response = ScenarioContext.Current.Response();
            var content = response.Content;

            var model = JsonConvert.DeserializeObject<List<T>>(content);


            CollectionAssert.AllItemsAreNotNull(model);
            CollectionAssert.AllItemsAreInstancesOfType(model, typeof(T));
            Assert.AreNotEqual(0, model.Count);

            var reflPropriedade = typeof(T).GetProperty(propriedade, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (reflPropriedade == null)
                throw new InvalidOperationException(string.Format("Propriedade {0} não encontrada no objeto {1}", propriedade, typeof(T).Name));
            foreach (var item in model)
            {
                //executar Busca Propriedade
                var propriedadeObj = reflPropriedade.GetValue(item);

                if (valor == null)
                    Assert.IsNull(propriedadeObj);
                else
                    Assert.AreEqual(valor, propriedadeObj.ToString());
            }
        }



        private static void AdicionarAutorizacao(RestRequest request)
        {
            var condominio = ScenarioContext.Current.Condominio();
            var objeto = ScenarioContext.Current.ObjetoAcesso();
            if (!string.IsNullOrWhiteSpace(condominio) && !string.IsNullOrWhiteSpace(objeto))
            {
                var token = AdicionarAutorizacao(condominio, objeto);

                request.AddHeader("Authorization", "Bearer " + token);
            }
        }

        public static string AdicionarAutorizacao(string condominio, params string[] objeto)
        {
            var token = ""; //TokenAcesso.GerarTokenJwt(condominio, objeto);

            return token;
        }

    }
}
