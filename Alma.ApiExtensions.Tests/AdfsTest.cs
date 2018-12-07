using Alma.ApiExtensions.Security;
using Microsoft.IdentityModel.Tokens.Saml;
using System;
using Xunit;

namespace Alma.Core.Security.Tests
{
    public class AdfsTest
    {
        [Fact]
        public async void TestarLoginAdfsSaml()
        {
            var username = "test@accenture.com"; //just to test failure
            var password = "anonymous";
            var audience = "audience";
            var authority = new Uri("https://federation-sts.accenture.com");

            var provider = new AdfsProvider();

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                //Change this test to use a valid password to do a sucessfull auth test
                var token = await provider.GetSamlToken(authority, audience, username, password) as SamlSecurityToken;
            });
            //Assert.NotNull(token);
            //Assert.NotNull(token.Assertion);
        }
    }
}
