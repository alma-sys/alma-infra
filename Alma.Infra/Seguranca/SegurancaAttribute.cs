using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Net.Http;
using Alma.Infra.Dados;
using Alma.Infra.Dominio.Entidades;
using System.Security.Claims;

namespace Alma.Infra.Seguranca
{
    public class SegurancaAttribute: AuthorizeAttribute
    {
        private string[] permissoes { get; set; }

        public SegurancaAttribute()
        {
            permissoes = new string[0];
        }

        public SegurancaAttribute(params string[] permissoes)
        {
            this.permissoes = permissoes;
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (!actionContext.RequestContext.Principal.Identity.IsAuthenticated)
                return false;

            var identity = actionContext.RequestContext.Principal.Identity as ClaimsIdentity;
            var permissoesUsuario = identity.Claims?.Where(c => c.Type == ClaimTypes.Role).Select(c=> c.Value).ToList();

            var temAcesso = false;

            if (permissoesUsuario.Contains(PermissoesBase.Root))
                return true;

            foreach (var permissao in permissoes)
            {
                temAcesso = temAcesso || permissoesUsuario.Contains(permissao);
            }

            return temAcesso;
        }
    }
}
