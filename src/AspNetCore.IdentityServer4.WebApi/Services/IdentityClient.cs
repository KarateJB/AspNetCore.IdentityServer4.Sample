using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core.Models.Config.WebApi;
using AspNetCore.IdentityServer4.Core.Models.Enum;
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
        private const int DEFAULT_REFRESH_DISCOVERY_DOC_DURATION = 24;
        private const string DEFAULT_SECRET = "secret";
        private string DEFAULT_CLIENT_ID = AuthClientEnum.PolicyBasedBackend.ToString();

        private readonly ILogger<IdentityClient> logger;
        private readonly IHttpClientFactory httpClientFactory = null;

        private readonly Semaphore semaphore = null;
        private readonly DiscoveryCache discoCacheClient = null;
        private DiscoveryDocumentResponse discoResponse = null;

        private readonly AppSettings appSettings = null;
        private readonly string remoteServiceBaseUrl = string.Empty;
        private readonly string clientId = string.Empty;
        private readonly string secret = string.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="logger">Logger</param>
        /// <param name="httpClientFactory">HttpClientFactory</param>
        public IdentityClient(
            IOptions<AppSettings> configuration,
            ILogger<IdentityClient> logger,
            IHttpClientFactory httpClientFactory)
        {
            this.appSettings = configuration.Value;
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.remoteServiceBaseUrl = this.appSettings.Host.AuthServer;
            this.semaphore = new Semaphore(1, 1);

            #region Set variables

            this.secret = DEFAULT_SECRET;
            this.clientId = this.appSettings?.AuthOptions?.ClientId ?? DEFAULT_CLIENT_ID;
            #endregion

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
            discoCacheClient.CacheDuration = TimeSpan.FromHours(this.appSettings.AuthOptions?.RefreshDiscoveryDocDuration ?? DEFAULT_REFRESH_DISCOVERY_DOC_DURATION);
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
                new KeyValuePair<string, string>("client_secret", this.secret),
                new KeyValuePair<string, string>("client_id", this.clientId),
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
                ClientId = this.clientId,
                ClientSecret = this.secret,
                UserName = userName,
                Password = password,
                //Scope = "MyBackendApi1 openid email" // "openid" is must if request for any IdentityResource
            });

            // Release the Mutex.
            this.semaphore.Release(1);

            // (Optional) Force refreshing dicovery document on next request to get the DiscoveryResponse
            // this.discoCacheClient.Refresh();

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
                ClientId = this.clientId,
                ClientSecret = this.secret,
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
                ClientId = this.clientId,
                ClientSecret = this.secret,
                Token = token
            });

            return revokeResposne;
        }

        /// <summary>
        /// Get JWKs
        /// </summary>
        /// <returns>JsonWebKeySetResponse</returns>
        /// <remarks>
        /// JWKs is available on https://auth_server/.well-known/openid-configuration/jwks
        /// </remarks>
        public async Task<JsonWebKeySetResponse> GetJwksAsync()
        {
            // Use Cached Discovery Document
            this.discoResponse = await this.discoverCachedDocumentAsync();

            var httpClient = this.httpClientFactory.CreateClient(HttpClientNameFactory.AuthHttpClient);

            JsonWebKeySetResponse jwksResponse = httpClient.GetJsonWebKeySetAsync(
                    new JsonWebKeySetRequest
                    {
                        Address = discoResponse.JwksUri,
                        ClientId = this.clientId,
                        ClientSecret = this.secret
                    }).Result;

            return jwksResponse;
        }

        /// <summary>
        /// Refresh cached discovery document of Idsrv4 on next request for DiscoveryResponse
        /// </summary>
        public async Task RefreshDiscoveryDocAsync()
        {
            this.discoCacheClient.Refresh();
            _ = await this.discoverCachedDocumentAsync();
            await Task.CompletedTask;
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
