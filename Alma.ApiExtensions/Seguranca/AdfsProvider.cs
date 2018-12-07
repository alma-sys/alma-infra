using Microsoft.IdentityModel.Tokens.Saml;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Alma.ApiExtensions.Security
{
    class AdfsProvider : IAdfsProvider
    {
        public AdfsProvider()
        {

        }


        public async Task<JwtSecurityToken> GetBearerTokenByCode(Uri adfs_uri, string code, string client_id, string audience, string redirect_uri, string resource)
        {
            var post = new HttpClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>() {
                { "grant_type", "authorization_code" },
                { "client_id", client_id },
                { "code", code },
                { "redirect_uri", redirect_uri },
                { "resource", resource },
            });

            var response = await post.PostAsync($"{adfs_uri.ToString()}/adfs/oauth2/token", content);
            var jwt_str = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<IDictionary<string, string>>(jwt_str);


            if (obj.ContainsKey("error"))
                throw new UnauthorizedAccessException("Unable to authorize the user on this resource.", new ApplicationException("ADFS ERROR: " + jwt_str));
            else
            {

                var h = new JwtSecurityTokenHandler();
                return h.ReadJwtToken(obj["access_token"]);
            }
        }

        /// <summary>
        /// Get a SamlToken by username and password
        /// </summary>
        /// <param name="adfs_uri">Full url with protocol</param>
        /// <param name="audience"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<SamlSecurityToken> GetSamlToken(Uri adfs_uri, string audience, string username, string password)
        {

            var usernameMixedEndpoint = $"{adfs_uri.ToString()}adfs/services/trust/13/usernamemixed";

            var rst = BuildRst(usernameMixedEndpoint, username, password, audience);

            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, usernameMixedEndpoint)
                {
                    Content = new StringContent(rst, Encoding.UTF8, "application/soap+xml")
                };
                var response = await httpClient.SendAsync(request);

                var xDoc = XDocument.Load(new StreamReader(await response.Content.ReadAsStreamAsync()));

                var unwrappedResponse = xDoc.Descendants((XNamespace)"http://docs.oasis-open.org/ws-sx/ws-trust/200512" + "RequestedSecurityToken")
                    .FirstOrDefault()?
                    .FirstNode as XElement;

                if (unwrappedResponse != null)
                {
                    var serializer = new SamlSerializer();
                    var token = serializer.ReadAssertion(new XmlTextReader(new StringReader(unwrappedResponse.ToString())));
                    return new SamlSecurityToken(token);
                }
                else
                    throw new UnauthorizedAccessException();
            }
        }


        private static string BuildRst(string usernameMixedEndpoint, string userName, string password, string audience)
        {
            var messageId = Guid.NewGuid();
            var tokenId = Guid.NewGuid();
            return string.Format(
                WsTrustSaml11TokenRstTemplate,
                messageId,
                usernameMixedEndpoint,
                tokenId,
                userName,
                password,
                audience);
        }


        const string WsTrustSaml11TokenRstTemplate =
        @"<s:Envelope 
            xmlns:s=""http://www.w3.org/2003/05/soap-envelope"" 
            xmlns:a=""http://www.w3.org/2005/08/addressing"" 
            xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
            <s:Header>
                <a:Action s:mustUnderstand=""1"">http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue</a:Action>
                <a:MessageID>urn:uuid:{0}</a:MessageID>
                <a:ReplyTo>
                    <a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address>
                </a:ReplyTo>
                <a:To s:mustUnderstand=""1"">{1}</a:To>
                <o:Security s:mustUnderstand=""1"" 
                    xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                    <o:UsernameToken u:Id=""uuid-{2}-7"">
                        <o:Username>{3}</o:Username>
                        <o:Password o:Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">{4}</o:Password>
                    </o:UsernameToken>
                </o:Security>
            </s:Header>
            <s:Body>
                <trust:RequestSecurityToken 
                    xmlns:trust=""http://docs.oasis-open.org/ws-sx/ws-trust/200512"">
                    <wsp:AppliesTo 
                        xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy"">
                        <a:EndpointReference>
                            <a:Address>{5}</a:Address>
                        </a:EndpointReference>
                    </wsp:AppliesTo>
                    <trust:KeyType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer</trust:KeyType>
                    <trust:RequestType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Issue</trust:RequestType>
                </trust:RequestSecurityToken>
            </s:Body>
        </s:Envelope>";


    }
}
