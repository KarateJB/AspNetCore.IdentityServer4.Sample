using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace AspNetCore.IdentityServer4.WebApi.Services
{
    public interface IIdentityClient : IDisposable
    {
        /// <summary>
        /// Get token by form data (Grant Type: password)
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <returns>HttpResponseMessage</returns>
        Task<HttpResponseMessage> GetTokenByFormDataAsync(string userName, string password);

        /// <summary>
        /// Sign in
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <returns>TokenResponse</returns>
        Task<TokenResponse> SignInAsync(string userName, string password);

        /// <summary>
        /// Get User's information by Access Token
        /// </summary>
        /// <param name="accessToken">Access Token</param>
        /// <returns>UserInfoReponse</returns>
        Task<UserInfoResponse> GetUserInfoAsync(string accessToken);

        /// <summary>
        /// Revoke Reference|Refresh token
        /// </summary>
        /// <param name="token">Reference|Refresh Token</param>
        /// <returns>TokenRevocationResponse</returns>
        Task<TokenRevocationResponse> RevokeTokenAsync(string token);
    }
}
