using Alma.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.Linq;

namespace Alma.ApiExtensions.Filters
{
    public class ValidationFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled) return;
            if (context.Exception is ValidacaoException validacoes)
            {
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

                context.ExceptionHandled = true; // mark exception as handled
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.StatusCode = 418;
                context.Result = new JsonResult(retorno, config)
                {
                    StatusCode = 418
                };
            }
        }
    }


}
