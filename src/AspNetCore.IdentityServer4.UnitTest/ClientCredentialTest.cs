using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using NUnit.Framework;

namespace AspNetCore.IdentityServer4.UnitTest
{
    public class TestResourceApi
    {
        private const string SECRETKEY = "secret";
        private const string CLIENTID = "Resources";
        private readonly string remoteServiceBaseUrl = "https://localhost:6001";
        private DiscoveryResponse discoResponse = null;
        private HttpClient httpClient = null;

        public TestResourceApi()
        {
            this.httpClient = new HttpClient();
        }

        [Test]
        public async Task TestSend()
        {
            #region Get access token

            if (this.discoResponse == null)
            {
                this.discoResponse = await this.DiscoverDocumentAsync();
            }

            var tokenResponse = await this.httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = this.discoResponse.TokenEndpoint,
                ClientId = CLIENTID,
                ClientSecret = SECRETKEY,
                Scope = "MyBackendApi2"
            });

            if (tokenResponse.IsError)
            {
                Assert.Fail(tokenResponse.Error);
            }
            #endregion

            #region Call protected API

            // Test with correct access token
            this.httpClient.SetBearerToken(tokenResponse.AccessToken);

            // Test with incorrect access token
            ////this.httpClient.SetBearerToken(tokenResponse.AccessToken.Remove(tokenResponse.AccessToken.Length - 1, 1));

            var response = await this.httpClient.GetAsync("https://localhost:5001/api/DemoPolicyBased/Everyone/Get");

            if (!response.IsSuccessStatusCode)
            {
                Assert.Fail(response.StatusCode.ToString());
            }
            else
            {
                Assert.True(true);
            }
            #endregion
        }

        /// <summary>
        /// Discover document
        /// </summary>
        /// <returns>DiscoveryResponse</returns>
        private async Task<DiscoveryResponse> DiscoverDocumentAsync()
        {
            var isRequireHttps = this.remoteServiceBaseUrl.StartsWith("https");

            this.discoResponse = await this.httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = this.remoteServiceBaseUrl,
                Policy =
                    {
                        RequireHttps = isRequireHttps,
                        ValidateIssuerName = true // Diable it if the issuer's name on Auth Server is not equal to the client's target
                    }
            });

            if (this.discoResponse.IsError)
            {
                throw new Exception(this.discoResponse.Error);
            }

            return this.discoResponse;
        }
    }
}