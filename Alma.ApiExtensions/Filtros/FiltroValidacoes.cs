using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;
using Alma.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Alma.ApiExtensions.Filtros
{
    public class FiltroValidacoes : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is ValidacaoException)
            {
                var validacoes = (ValidacaoException)context.Exception;

                var erros = validacoes.Errors.Select(t => new { Campo = t.Key, Mensagem = t.Value }).ToArray();
                object retorno;
                if (erros.Length > 0)
                    retorno = new
                    {
                        mensagem = validacoes.Message,
                        erros = erros
                    };
                else
                    retorno = new
                    {
                        mensagem = validacoes.Message
                    };

                var config = new JsonSerializerSettings();
                config.ContractResolver = new CamelCasePropertyNamesContractResolver();
                config.Culture = new CultureInfo("pt-BR");

                context.Response = new HttpResponseMessage((HttpStatusCode)418)
                {
                    ReasonPhrase = "Erro de validacao",
                    Content = new StringContent(
                        JsonConvert.SerializeObject(retorno, config),
                        Encoding.UTF8,
                        "application/json")
                };
            }
        }
    }
}
