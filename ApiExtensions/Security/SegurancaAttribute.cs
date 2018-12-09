using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Alma.ApiExtensions.Security
{
    //public static class SecurityExtension
    //{

    //    public static void UseSecurityAuthorization(this IServiceCollection services)
    //    {
    //        var permissoes = new string[] { };
    //        //pegar attributos das classes e adicionar as autorizações. 
    //        foreach (var permissao in permissoes)
    //        {
    //            services.AddAuthorization(options =>
    //            {
    //                options.AddPolicy("Seguranca_" + permissao,
    //                                  policy => policy.Requirements.Add(new SegurancaRequirement(permissao)));
    //            });
    //        }
    //        throw new NotImplementedException();
    //    }
    //}

    public sealed class AuthorizationAttribute : AuthorizeAttribute
    {
        public AuthorizationAttribute(params string[] roles)
            : base()
        {
            this.Roles = roles != null && roles.Length > 0 ? string.Join(",", roles) : null;
        }
    }


    //public class SegurancaRequirement : AuthorizationHandler<SegurancaRequirement>, IAuthorizationRequirement
    //{
    //    public SegurancaRequirement(params string[] permissao)
    //    {
    //        this.Permissoes = permissao;
    //    }

    //    public string[] Permissoes { get; private set; }

    //    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SegurancaRequirement requirement)
    //    {
    //        if (context.User == null || context.User.Identity == null ||
    //               !context.User.Identity.IsAuthenticated)
    //        {
    //            context.Fail();
    //            return Task.CompletedTask;
    //        }

    //        var identity = context.User.Identity as ClaimsIdentity;
    //        var permissoesUsuario = identity.Claims?.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

    //        var temAcesso = false;

    //        if (permissoesUsuario.Contains(PermissoesBase.Root))
    //            temAcesso = true;
    //        else
    //        {
    //            temAcesso = permissoesUsuario.Count == 0; //começa com true se a api não tiver permissão específica.
    //            foreach (var permissao in Permissoes)
    //            {
    //                temAcesso = temAcesso || permissoesUsuario.Contains(permissao);
    //            }
    //        }

    //        if (temAcesso)
    //        {
    //            context.Succeed(requirement);
    //        }
    //        else
    //        {
    //            context.Fail();
    //        }

    //        return Task.CompletedTask;
    //    }
    //}
}

