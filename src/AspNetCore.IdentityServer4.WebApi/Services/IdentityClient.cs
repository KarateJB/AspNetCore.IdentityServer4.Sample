using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core.Models.Config.WebApi;
using AspNetCore.IdentityServer4.Core.Utils.Factory;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCore.IdentityServer4.WebApi.Services
{
    /// <summary>
    /// IdentityClient
    /// </summary>
    public class IdentityClient : IIdentityClient
    {
        private const string SECRETKEY = "secret";
        private const string CLIENTID = "PolicyBasedBackend"; // Or "MyBackend"
        private readonly AppSettings configuration = null;
        private readonly ILogger<IdentityClient> logger;
        private readonly IHttpClientFactory httpClientFactory = null;
        private readonly string remoteServiceBaseUrl = string.Empty;
        private readonly Semaphore semaphore = null;
        private readonly DiscoveryCache discoCacheClient = null;
        private DiscoveryDocumentResponse discoResponse = null;

        public IdentityClient(
            IOptions<AppSettings> configuration,
            ILogger<IdentityClient> logger,
            IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration.Value;
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.remoteServiceBaseUrl = this.configuration.Host.AuthServer;
            this.semaphore = new Semaphore(1, 1);

            #region Create Discovery Cache client

            var discoPolicy = this.remoteServiceBaseUrl.StartsWith("https") ?
                null :
                new DiscoveryPolicy
                {
                    RequireHttps = false,
                };

            this.discoCacheClient = new DiscoveryCache(
                this.remoteServiceBaseUrl,
                () => this.httpClientFactory.CreateClient(HttpClientNameFactory.AuthHttpClient),
                discoPolicy);

            // Set cache duration
            discoCacheClient.CacheDuration = TimeSpan.FromHours(8);
            #endregion
        }

        /// <summary>
        /// Get token by form data (Grant Type: password)
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> GetTokenByFormDataAsync(string userName, string password)
        {
            var httpClient = this.httpClientFactory.CreateClient(HttpClientNameFactory.AuthHttpClient);
            var endpoint = new Uri(string.Concat(this.remoteServiceBaseUrl, "/connect/token"));

            // Set Http request's Accept header
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Const
            const string grantType = "password";
            const string scope = "MyBackendApi1 offline_access";

            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", grantType),
                new KeyValuePair<string, string>("client_secret", SECRETKEY),
                new KeyValuePair<string, string>("client_id", CLIENTID),
                new KeyValuePair<string, string>("scope", scope),
                new KeyValuePair<string, string>("username", userName),
                new KeyValuePair<string, string>("password", password)
            });

            var response = await httpClient.PostAsync(endpoint, formData);
            return response;
        }

        /// <summary>
        /// Sign in
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <returns>TokenResponse</returns>
        public async Task<TokenResponse> SignInAsync(string userName, string password)
        {
            // if (this.discoResponse == null)
            // {
            //     this.discoResponse = await this.discoverDocumentAsync();
            // }

            // Use Cached Discovery Document
            this.discoResponse = await this.discoverCachedDocumentAsync();


            var httpClient = this.httpClientFactory.CreateClient(HttpClientNameFactory.AuthHttpClient);

            // Wait until it is safe to enter.
            this.semaphore.WaitOne();

            TokenResponse tokenResponse = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = this.discoResponse.TokenEndpoint,
                ClientId = CLIENTID,
                ClientSecret = SECRETKEY,
                UserName = userName,
                Password = password,
                //Scope = "MyBackendApi1 openid email" // "openid" is must if request for any IdentityResource
            });

            // Release the Mutex.
            this.semaphore.Release(1);

            return tokenResponse;
        }

        /// <summary>
        /// Get User's information by Access Token
        /// </summary>
        /// <param name="accessToken">Access Token</param>
        /// <returns>UserInfoReponse</returns>
        public async Task<UserInfoResponse> GetUserInfoAsync(string accessToken)
        {
            // Use Cached Discovery Document
            this.discoResponse = await this.discoverCachedDocumentAsync();

            var httpClient = this.httpClientFactory.CreateClient(HttpClientNameFactory.AuthHttpClient);
            UserInfoResponse userInfoResponse = await httpClient.GetUserInfoAsync(new UserInfoRequest()
            {
                Address = this.discoResponse.UserInfoEndpoint,
                Token = accessToken
            });

            return userInfoResponse;
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh-token</param>
        /// <returns>TokenResponse</returns>
        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            // Use Cached Discovery Document
            this.discoResponse = await this.discoverCachedDocumentAsync();

            var httpClient = this.httpClientFactory.CreateClient(HttpClientNameFactory.AuthHttpClient);
            TokenResponse tokenResponse = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = this.discoResponse.TokenEndpoint,
                ClientId = CLIENTID,
                ClientSecret = SECRETKEY,
                RefreshToken = refreshToken
            });

            return tokenResponse;
        }

        /// <summary>
        /// Revoke Reference|Refresh token
        /// </summary>
        /// <param name="token">Reference|Refresh Token</param>
        /// <returns>TokenRevocationResponse</returns>
        public async Task<TokenRevocationResponse> RevokeTokenAsync(string token)
        {
            // Use Cached Discovery Document
            this.discoResponse = await this.discoverCachedDocumentAsync();

            var httpClient = this.httpClientFactory.CreateClient(HttpClientNameFactory.AuthHttpClient);
            TokenRevocationResponse revokeResposne = await httpClient.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = this.discoResponse.RevocationEndpoint,
                ClientId = CLIENTID,
                ClientSecret = SECRETKEY,
                Token = token
            });

            return revokeResposne;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }

        private async Task<DiscoveryDocumentResponse> discoverDocumentAsync()
        {
            var httpClient = this.httpClientFactory.CreateClient(HttpClientNameFactory.AuthHttpClient);
            DiscoveryDocumentResponse discoResponse = null;

            if (this.remoteServiceBaseUrl.StartsWith("https"))
            {
                discoResponse = await httpClient.GetDiscoveryDocumentAsync(this.remoteServiceBaseUrl);
            }
            else
            {
                discoResponse = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
                {
                    Address = this.remoteServiceBaseUrl,
                    Policy =
                {
                    RequireHttps = false // default: true
                }
                });
            }

            if (discoResponse.IsError)
            {
                throw new Exception(discoResponse.Error);
            }

            return discoResponse;
        }

        private async Task<DiscoveryDocumentResponse> discoverCachedDocumentAsync()
        {
            DiscoveryDocumentResponse discoResponse = null;
            
            discoResponse = await this.discoCacheClient.GetAsync();

            if (discoResponse.IsError)
            {
                throw new Exception(discoResponse.Error);
            }

            return discoResponse;
        }
    }
}
