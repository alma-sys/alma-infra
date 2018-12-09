using Alma.Common;
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
            if (context.Exception is ValidationException validationList)
            {
                var erros = validationList.Errors.Select(t => new { Campo = t.Key, Mensagem = t.Value }).ToArray();
                object retorno;
                if (erros.Length > 0)
                    retorno = new
                    {
                        mensagem = validationList.Message,
                        erros = erros
                    };
                else
                    retorno = new
                    {
                        mensagem = validationList.Message
                    };

                var config = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Culture = CultureInfo.CurrentCulture
                };

                context.ExceptionHandled = true; // mark exception as handled
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.StatusCode = 418;
                context.Result = new JsonResult(retorno, config)
                {
                    StatusCode = 418 // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/418
                };
            }
        }
    }


}
