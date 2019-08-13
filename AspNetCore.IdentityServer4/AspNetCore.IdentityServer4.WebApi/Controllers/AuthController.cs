using System.Net;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Auth.Models;
using AspNetCore.IdentityServer4.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AspNetCore.IdentityServer4.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityClient auth = null;

        public AuthController(
            IIdentityClient id4Client)
        {
            this.auth = id4Client;
        }

        [HttpPost("GetToken")]
        [AllowAnonymous]
        public async Task<JObject> GetToken(LdapUser user)
        {
            var response = await this.auth.GetTokenByFormDataAsync(user.Username, user.Password);

            if (response.IsSuccessStatusCode == true)
            {
                var strResult = await response.Content.ReadAsStringAsync();
                return JObject.Parse(strResult);
            }

            this.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return null;
        }

        [HttpPost("SignIn")]
        [AllowAnonymous]
        public async Task<JObject> SignIn(LdapUser user)
        {
            var tokenResponse = await this.auth.SignInAsync(user.Username, user.Password);


            if (!tokenResponse.IsError)
            {
                return tokenResponse.Json;
            }

            this.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return null;
        }

        [HttpPost("UserInfo")]
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

            var userInfoResponse = await this.auth.GetUserInfoAsync(accessToken);

            if (!userInfoResponse.IsError)
            {
                return userInfoResponse.Json;
            }

            this.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return null;
        }
    }
}
