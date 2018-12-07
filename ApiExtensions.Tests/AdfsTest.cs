using Alma.ApiExtensions.Security;
using Microsoft.IdentityModel.Tokens.Saml;
using System;
using System.Threading;
using Xunit;

namespace Alma.Core.Security.Tests
{
    public class AdfsTest
    {
        private readonly string username;
        private readonly string password;
        private readonly string audience;
        private readonly Uri authority;

        public AdfsTest()
        {
            this.username = "marcos.junior@company.com.br";
            this.password = "----";
            this.audience = "https://myapplication.company.com.br/";
            this.authority = new Uri("https://adfs.company.com.br");

        }

        [Fact(Skip = "Change this test to use a valid password to do a sucessfull auth test")]
        public async void TestarLoginAdfsSaml()
        {

            var provider = new AdfsProvider();

            //await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            //{
            var token = await provider.GetSamlToken(authority, audience, username, password) as SamlSecurityToken;
            // });
            Assert.NotNull(token);
            Assert.NotNull(token.Assertion);
        }

        [Fact(Skip = "Change this test to use a valid password to do a sucessfull auth test")]
        public async void TestarRenewAdfsSaml()
        {
            var provider = new AdfsProvider();

            //await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            //{
            var token = await provider.GetSamlToken(authority, audience, username, password) as SamlSecurityToken;
            Thread.Sleep(10000);
            var renewed = await provider.RenewSamlToken(authority, audience, token.Assertion.AssertionId);

            //});
            Assert.NotNull(token);
            Assert.NotNull(token.Assertion);
        }
    }
}
