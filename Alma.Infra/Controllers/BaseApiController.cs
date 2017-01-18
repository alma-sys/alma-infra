using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Alma.Infra.Controllers
{
    public class BaseApiController : ApiController
    {
        private ClaimsIdentity identity
        {
            get
            {
                return User.Identity as ClaimsIdentity;
            }
        }
        protected String Usuario { get { return identity.Name; } }

        protected Int32? Condominio
        {
            get
            {
                var claim = identity.Claims.SingleOrDefault(c => c.Type == "condominio");
                var id = 0;
                if (Int32.TryParse(claim.Value, out id))
                    return id;

                return null;
            }
        }

        protected Int32? Bloco
        {
            get
            {
                var claim = identity.Claims.SingleOrDefault(c => c.Type == "bloco");
                var id = 0;
                if (Int32.TryParse(claim.Value, out id))
                    return id;

                return null;
            }
        }

        protected Int32? Unidade
        {
            get
            {
                var claim = identity.Claims.SingleOrDefault(c => c.Type == "unidade");
                var id = 0;
                if (Int32.TryParse(claim.Value, out id))
                    return id;

                return null;
            }
        }
    }
}
