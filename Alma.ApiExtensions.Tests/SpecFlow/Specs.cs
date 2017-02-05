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

namespace Alma.ApiExtensions.Testes.SpecFlow
{
    public abstract partial class GenericSteps
    {

        public virtual ScenarioContext ScenarioContext { get; private set; }
        public GenericSteps(ScenarioContext scenarioContext)
        {
            if (scenarioContext == null) throw new ArgumentNullException(nameof(scenarioContext));
            this.ScenarioContext = scenarioContext;
        }


        [Given(@"o método http é '(.*)'")]
        public virtual void EOMetodoHttpE(string p0)
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

            ScenarioContext.HttpMethod(metodo);
        }

        [Given(@"a seguinte lista de argumentos do tipo '(.*)':")]
        public virtual void DadoASeguinteListaDeArgumentos(string nomeTipo, Table table)
        {
            var tipo = LocalizarTipo(nomeTipo);

            var reflValidarResposta = typeof(TableHelperExtensionMethods).GetMethod("CreateSet", new[] { typeof(Table) }).GetGenericMethodDefinition();
            var genericMethod = reflValidarResposta.MakeGenericMethod(tipo);
            var arg = genericMethod.Invoke(null, new[] { table });

            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var json = JsonConvert.SerializeObject(arg, Formatting.Indented, jsonSerializerSettings);

            ScenarioContext.JsonArgumento(json);
            ScenarioContext.ListaArgumentos(table);
        }

        [Given(@"que a url do endpoint é '(.*)'")]
        public virtual void DadoQueAUrlDoEndpointE(string p0)
        {
            ScenarioContext.Endpoint(p0);
        }

        [Given(@"os seguintes parâmetros de url:")]
        public virtual void DadoOsSeguintesParametrosDeUrl(Table table)
        {
            ScenarioContext.ParametroUrl(table);
        }

        [Given(@"informei o parâmetro de url '(.*)' com o id do cenário")]
        public virtual void DadoInformeiOParametroDeUrlComOIdDoCenario(string parametro)
        {
            Int64 id = ScenarioContext.IdEntidade();
            AdicionaParametroUrl(parametro, id.ToString());
        }

        [Given(@"informei o parâmetro de url '(.*)' com o valor '(.*)'")]
        public virtual void DadoInformeiOParametroDeUrlComOValor(string parametro, string valor)
        {
            AdicionaParametroUrl(parametro, valor);
        }

        private void AdicionaParametroUrl(String parametro, String valor)
        {
            var table = ScenarioContext.ParametroUrl();
            if (table == null)
                table = new Table("Field", "Value");

            table.AddRow(parametro, valor);
            ScenarioContext.ParametroUrl(table);
        }

        [Given(@"o seguinte argumento do tipo '(.*)':")]
        public virtual void EOsSeguintesValores(string nomeTipo, Table table)
        {
            var tipo = LocalizarTipo(nomeTipo);

            var reflValidarResposta = typeof(TableHelperExtensionMethods).GetMethod("CreateInstance", new[] { typeof(Table) }).GetGenericMethodDefinition();
            var genericMethod = reflValidarResposta.MakeGenericMethod(tipo);
            var arg = genericMethod.Invoke(null, new[] { table });

            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var json = JsonConvert.SerializeObject(arg, Formatting.Indented, jsonSerializerSettings);

            ScenarioContext.JsonArgumento(json);
            ScenarioContext.TipoModel(nomeTipo);
            ScenarioContext.Argumento(table);
        }

        [Then(@"statuscode da resposta deverá ser '(.*)'")]
        public virtual void EntaoStatuscodeDaRespostaDeveraSer(string p0)
        {
            var response = ScenarioContext.Response();

            Assert.AreEqual(p0, response.StatusCode.ToString(),
                response.StatusCode == HttpStatusCode.InternalServerError ?
                response.Content : null);
        }

        protected virtual Type LocalizarTipo(string nomeTipo)
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

        [When(@"chamar o servico")]
        public virtual void QuandoChamarOServico()
        {
            var url = (String)ScenarioContext.Endpoint();
            var endpoint = Config.HostBase.ToString();
            if (!endpoint.EndsWith("/"))
                endpoint += "/";
            endpoint += url.Substring(url.StartsWith("/") ? 1 : 0);

            ExecutarRequest(endpoint);
        }

        [Then(@"uma resposta do tipo '(.*)' deve ser retornada com os seguintes valores:")]
        public virtual void EntaoUmaRespostaDoTipoXSerRetornadaComOsSeguintesValores(string tipoModel, Table table)
        {
            var tipoModelType = LocalizarTipo(tipoModel);
            ScenarioContext.TipoModel(tipoModel);
            try
            {
                //executar ValidarResposta generica
                var reflValidarResposta = typeof(GenericSteps).GetMethod(nameof(ValidarResposta), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetGenericMethodDefinition();
                var genericMethod = reflValidarResposta.MakeGenericMethod(tipoModelType);
                genericMethod.Invoke(this, new[] { table });
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
        public virtual void EntaoUmaRespostaDoTipoXSerRetornadaComUmItemComOsSeguintesValores(string tipoModel, Table table)
        {
            var tipoModelType = LocalizarTipo(tipoModel);
            ScenarioContext.TipoModel(tipoModel);
            try
            {
                //executar ValidarResposta generica
                var reflValidarResposta = typeof(GenericSteps).GetMethod(nameof(ValidarRespostaListaComUmItem), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetGenericMethodDefinition();
                var genericMethod = reflValidarResposta.MakeGenericMethod(tipoModelType);
                genericMethod.Invoke(this, new[] { table });
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
        public virtual void EntaoAPropriedadeDeTodosOsItensDeveSer(string propriedade, string valor)
        {
            var tipoModel = ScenarioContext.TipoModel();
            if (string.IsNullOrWhiteSpace(tipoModel))
                throw new InvalidOperationException("Validar se a lista foi retornada com dados antes de validar propriedades.");
            var tipoModelType = LocalizarTipo(tipoModel);

            try
            {
                //executar ValidarResposta generica
                var reflValidarResposta = typeof(GenericSteps).GetMethod(nameof(ValidarRespostaListaComTodosOsItensComPropriedade), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetGenericMethodDefinition();
                var genericMethod = reflValidarResposta.MakeGenericMethod(tipoModelType);
                genericMethod.Invoke(this, new[] { propriedade, valor });
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
        public virtual void EntaoUmaRespostaComUmaListaDoTipoXSerRetornadaComItens(string tipoModel)
        {
            var tipoModelType = LocalizarTipo(tipoModel);
            ScenarioContext.TipoModel(tipoModel);
            try
            {
                //executar ValidarResposta generica
                var reflValidarResposta = typeof(GenericSteps).GetMethod(nameof(ValidarRespostaListaComDados), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetGenericMethodDefinition();
                var genericMethod = reflValidarResposta.MakeGenericMethod(tipoModelType);
                genericMethod.Invoke(this, new object[] { });
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
        public virtual void EntaoUmaPropriedadeXComUmaListaDeY(string propriedade, string tipoLista, Table table)
        {
            try
            {

                var tipoModel = ScenarioContext.TipoModel();
                var tipoModelType = LocalizarTipo(tipoModel);


                var response = ScenarioContext.Response();
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
                genericMethod.Invoke(this, new[] { table, propriedadeObj });
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
        public virtual void DadoQueNaoTenhoUmaSessaoDeUsuarioAtiva()
        {
            ScenarioContext.ObjetoAcesso(null);
        }

        [Given(@"que tenho uma sessão de usuário ativa com alguma permissão no sistema")]
        public virtual void DadoQueTenhoUmaSessaoDeUsuarioAtivaNoCondominioComAlgumaPermissaoNoSistema()
        {
            DadoOPerfilDeAcessoEContemOObjeto("dummy");
        }

        [Given(@"que tenho uma sessão de usuário ativa com permissão ao objeto '(.*)'")]
        public virtual void DadoOPerfilDeAcessoEContemOObjeto(string objeto)
        {
            ScenarioContext.ObjetoAcesso(objeto);
        }

        /// <summary>
        /// Executa a mágica
        /// </summary>
        /// <param name="endpoint">Url da requisição</param>
        /// <returns> Objeto IRestResponse do Tipo Y </returns>
        private IRestResponse ExecutarRequest(String endpoint)
        {
            var url = ScenarioContext.MontaUrl(endpoint);

            var request = new RestRequest();

            request.Method = ScenarioContext.HttpMethod();

            request.Parameters.Clear();

            if (request.Method == Method.POST)
            {
                var json = ScenarioContext.JsonArgumento();
                if (!String.IsNullOrWhiteSpace(json))
                    request.AddParameter("application/json", json, ParameterType.RequestBody);
            }

            request.AddHeader("Accept", "application/json");
            AdicionarAutorizacao(request);

            var restClient = new RestClient(url);

            var response = restClient.Execute(request);

            ScenarioContext.Response(response);

            return response;
        }


        private String GetJsonArgumento<T>()
        {
            object arg = null;
            if (ScenarioContext.ListaArgumentos() != null)
            {
                arg = ScenarioContext.ListaArgumentos().CreateSet<T>();
            }
            else if (ScenarioContext.Argumento() != null)
            {
                arg = ScenarioContext.Argumento().CreateInstance<T>();
            }

            if (arg == null)
                return String.Empty;

            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var json = JsonConvert.SerializeObject(arg, Formatting.Indented, jsonSerializerSettings);

            return json;
        }

        public virtual void ValidarResposta<T>(Table table) where T : new()
        {
            var response = ScenarioContext.Response();
            var content = response.Content;

            var model = JsonConvert.DeserializeObject<T>(content);

            table.CompareToInstance(model);
        }

        public virtual void ValidarRespostaLista<T>(Table table) where T : new()
        {
            var response = ScenarioContext.Response();
            var content = response.Content;

            var model = JsonConvert.DeserializeObject<List<T>>(content);

            table.CompareToSet<T>(model);
        }

        public virtual void ValidarRespostaListaComDados<T>() where T : new()
        {
            var response = ScenarioContext.Response();
            var content = response.Content;

            var model = JsonConvert.DeserializeObject<List<T>>(content);

            CollectionAssert.AllItemsAreNotNull(model);
            CollectionAssert.AllItemsAreInstancesOfType(model, typeof(T));
            Assert.AreNotEqual(0, model.Count);
        }

        public virtual void ValidarRespostaListaComUmItem<T>(Table table) where T : new()
        {
            var response = ScenarioContext.Response();
            var content = response.Content;

            var model = JsonConvert.DeserializeObject<List<T>>(content);

            CollectionAssert.AllItemsAreNotNull(model);
            CollectionAssert.AllItemsAreInstancesOfType(model, typeof(T));
            Assert.AreEqual(1, model.Count);

            table.CompareToInstance(model.First());
        }
        public virtual void ValidarRespostaListaComTodosOsItensComPropriedade<T>(string propriedade, string valor) where T : new()
        {
            var response = ScenarioContext.Response();
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



        private void AdicionarAutorizacao(RestRequest request)
        {
            var objetos = ScenarioContext.ObjetoAcesso();
            if (!string.IsNullOrWhiteSpace(objetos))
            {
                var token = ObterTokenAutorizacao(objetos);
                if (!string.IsNullOrWhiteSpace(token))
                    request.AddHeader("Authorization", "Bearer " + token);
            }
        }

        public virtual string ObterTokenAutorizacao(params string[] objetos)
        {
            return null;
        }

    }
}
