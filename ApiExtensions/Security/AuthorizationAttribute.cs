using Microsoft.AspNetCore.Authorization;

namespace Alma.ApiExtensions.Security
{
    public sealed class AuthorizationAttribute : AuthorizeAttribute
    {
        public AuthorizationAttribute(params string[] roles)
            : base()
        {
            this.Roles = roles != null && roles.Length > 0 ? string.Join(",", roles) : null;
        }
    }

}

