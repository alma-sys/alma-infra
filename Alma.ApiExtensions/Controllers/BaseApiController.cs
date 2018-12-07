using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace Alma.ApiExtensions.Controllers
{
    [Controller]
    public abstract class BaseApiController : Controller
    {

        [ActionContext]
        public ActionContext ActionContext { get; set; }

        private ClaimsIdentity Identity
        {
            get
            {
                return ActionContext.HttpContext.User.Identity as ClaimsIdentity;
            }
        }
        protected string Usuario { get { return Identity.Name; } }

        protected T ObterValorToken<T>(string key)
        {
            var claim = Identity.Claims.SingleOrDefault(c => c.Type.ToLower() == key);
            if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
                return default(T);
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
            {
                int i = 0;
                if (int.TryParse(claim.Value, out i))
                    return (T)(object)i;
                else
                    return default(T);
            }
            else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
            {
                long i = 0;
                if (long.TryParse(claim.Value, out i))
                    return (T)(object)i;
                else
                    return default(T);
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)(object)claim.Value;
            }
            else
                throw new ArgumentException("Tipo não suportado", "T");

        }

    }
}
