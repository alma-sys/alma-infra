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
        public ActionContext ActionContext { get; internal set; }

        private ClaimsIdentity Identity
        {
            get
            {
                return ActionContext?.HttpContext?.User?.Identity as ClaimsIdentity;
            }
        }
        protected string UserName { get { return Identity?.Name; } }

        protected T GetUserTokenValue<T>(string key)
        {
            if (Identity == null)
                return default(T);

            var claim = Identity.Claims.SingleOrDefault(c => c.Type.ToLower() == key);
            if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
                return default(T);
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
            {
                if (int.TryParse(claim.Value, out int i))
                    return (T)(object)i;
                else
                    return default(T);
            }
            else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
            {
                if (long.TryParse(claim.Value, out long i))
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
