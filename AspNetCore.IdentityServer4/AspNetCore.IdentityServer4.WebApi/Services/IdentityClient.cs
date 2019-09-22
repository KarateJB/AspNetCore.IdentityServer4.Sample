using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.WebApi.Models;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace AspNetCore.IdentityServer4.WebApi.Services
{
    public class IdentityClient : IIdentityClient
    {
        public void Dispose()
        {
        }

        private const string SECRETKEY = "secret";
        private const string CLIENTID = "RoleBasedBackend";
        private readonly AppSettings configuration = null;
        private readonly HttpClient httpClient = null;
        private readonly string remoteServiceBaseUrl = string.Empty;

        public IdentityClient(
            IOptions<AppSettings> configuration,
            HttpClient httpClient)
        {
            this.configuration = configuration.Value;
            this.httpClient = httpClient;
            this.remoteServiceBaseUrl = this.configuration.Host.AuthServer;
        }

        /// <summary>
        /// Get token by form data (Grant Type: password)
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> GetTokenByFormDataAsync(string userName, string password)
        {
            var endpoint = new Uri(string.Concat(this.remoteServiceBaseUrl, "/connect/token"));

            // Set Http request's Accept header
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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

            var response = await this.httpClient.PostAsync(endpoint, formData);
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
            var discoResponse = await this.discoverDocumentAsync();

            TokenResponse tokenResponse = await this.httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = discoResponse.TokenEndpoint,
                ClientId = CLIENTID,
                ClientSecret = SECRETKEY,
                UserName = userName,
                Password = password,
                //Scope = "MyBackendApi1 openid email" // "openid" is must if request for any IdentityResource
            });

            return tokenResponse;
        }

        /// <summary>
        /// Get User's information by Access Token
        /// </summary>
        /// <param name="accessToken">Access Token</param>
        /// <returns>UserInfoReponse</returns>
        public async Task<UserInfoResponse> GetUserInfoAsync(string accessToken)
        {
            var discoResponse = await this.discoverDocumentAsync();

            UserInfoResponse userInfoResponse = await this.httpClient.GetUserInfoAsync(new UserInfoRequest()
            {
                Address = discoResponse.UserInfoEndpoint,
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
            var discoResponse = await this.discoverDocumentAsync();

            TokenResponse tokenResponse = await this.httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = discoResponse.TokenEndpoint,
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
            var discoResponse = await this.discoverDocumentAsync();

            TokenRevocationResponse revokeResposne = await this.httpClient.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = discoResponse.RevocationEndpoint,
                ClientId = CLIENTID,
                ClientSecret = SECRETKEY,
                Token = token
            });

            return revokeResposne;
        }

        private async Task<DiscoveryResponse> discoverDocumentAsync()
        {
            var discoResponse = await this.httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = this.remoteServiceBaseUrl,
                Policy =
                {
                    RequireHttps = true // default: true
                }
            });

            if (discoResponse.IsError)
            {
                throw new Exception(discoResponse.Error);
            }

            return discoResponse;
        }
    }
}
