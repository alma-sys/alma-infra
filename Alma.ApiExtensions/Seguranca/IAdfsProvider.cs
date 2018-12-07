using Microsoft.IdentityModel.Tokens.Saml;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Alma.ApiExtensions.Security
{
    public interface IAdfsProvider
    {
        Task<SamlSecurityToken> GetSamlToken(Uri adfs_uri, string audience, string username, string password);

        Task<JwtSecurityToken> GetBearerTokenByCode(Uri adfs_uri, string code, string client_id, string audience, string redirect_uri, string resource);
    }
}