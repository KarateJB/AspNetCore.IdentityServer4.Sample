using System.Net;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Auth.Models;
using AspNetCore.IdentityServer4.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AspNetCore.IdentityServer4.WebApi.Areas.Auth.Controllers
{
    /// <summary>
    /// Authenticiation controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> logger = null;
        private readonly IIdentityClient idsrvClient = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="idsrvClient">Idsrc client</param>
        public AuthController(
            ILogger<AuthController> logger,
            IIdentityClient idsrvClient)
        {
            this.logger = logger;
            this.idsrvClient = idsrvClient;
        }

        #region GetToken

        /// <summary>
        /// Get Access Token by form data
        /// </summary>
        /// <param name="user">LDAP user</param>
        /// <returns>JSON object</returns>
        [HttpPost("GetToken")]
        [AllowAnonymous]
        public async Task<JObject> GetToken(LdapUser user)
        {
            var response = await this.idsrvClient.GetTokenByFormDataAsync(user.Username, user.Password);

            if (response.IsSuccessStatusCode == true)
            {
                var strResult = await response.Content.ReadAsStringAsync();
                return JObject.Parse(strResult);
            }

            this.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return null;
        }
        #endregion

        #region SignIn

        /// <summary>
        /// Sign in and get Access Token
        /// </summary>
        /// <param name="user">LDAP user</param>
        /// <returns>JSON object</returns>
        [HttpPost("SignIn")]
        [AllowAnonymous]
        public async Task<JObject> SignIn(LdapUser user)
        {
            var tokenResponse = await this.idsrvClient.SignInAsync(user.Username, user.Password);
            this.HttpContext.Response.StatusCode = tokenResponse.IsError? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.OK;
            
            return tokenResponse.Json;
        }
        #endregion

        #region UserInfo

        /// <summary>
        /// Get user information from Access Token
        /// </summary>
        /// <param name="accessToken">Access Token</param>
        /// <returns>JSON object</returns>
        [HttpPost("UserInfo")]
        [AllowAnonymous]
        public async Task<JObject> UserInfo([FromBody] string accessToken)
        {
            /*
             * How to get Access token:
             * 1. Get the Access token from Header: "Authorization" by API parameter: "[FromHeader] string authorization"
             * 2. Get the Header: "Authorization"'s value from this.Request 
             * and parse it as following...
             */

            // string accessToken = string.Empty;
            // var authHeaderVal = this.Request.Headers["Authorization"];
            // if (!string.IsNullOrEmpty(authHeaderVal))
            // {
            //    accessToken = authHeaderVal.ToString().Replace("Bearer ", "").Replace("bearer ", "");
            // }

            var userInfoResponse = await this.idsrvClient.GetUserInfoAsync(accessToken);
            this.HttpContext.Response.StatusCode = userInfoResponse.IsError? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.OK;

            return userInfoResponse.Json;
        }
        #endregion

        #region RefreshToken

        /// <summary>
        /// Refresh token and get new tokens
        /// </summary>
        /// <param name="refreshToken">Refresh Token</param>
        /// <returns>JSON object with new tokens</returns>
        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<JObject> RefreshToken([FromBody] string refreshToken)
        {
            var tokenResponse = await this.idsrvClient.RefreshTokenAsync(refreshToken);
            this.HttpContext.Response.StatusCode = tokenResponse.IsError? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.OK;
            return tokenResponse.Json;
        }
        #endregion

        #region RevokeToken

        /// <summary>
        /// Revoke token (Can be Refresh token/Reference token)
        /// </summary>
        /// <param name="token">Refresh token or Reference token</param>
        /// <returns></returns>
        [HttpPost("RevokeToken")]
        [Authorize]
        public async Task<string> RevokeToken([FromBody] string token)
        {
            var revokeResponse = await this.idsrvClient.RevokeTokenAsync(token);
            this.HttpContext.Response.StatusCode = revokeResponse.IsError ? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.OK;
            return revokeResponse.Error;
        }
        #endregion
    }
}
