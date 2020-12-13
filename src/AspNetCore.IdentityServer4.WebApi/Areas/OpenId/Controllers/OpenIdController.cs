using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.WebApi.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.WebApi.Areas.Auth.Controllers
{
    /// <summary>
    /// Open ID controller
    /// </summary>
    [Route("[controller]")]
    [Authorize] // DONOT specify the AuthenticationSchemes here
    public class OpenIdController : Controller
    {
        private readonly ILogger logger = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        public OpenIdController(ILogger<OpenIdController> logger)
        {
            this.logger = logger;
        }

        #region Test
        [HttpGet("Test")]
        public async Task<IActionResult> Test()
        {
            /*
             * You can use this API(/OpenId/Test) for testing if the user has been authorized. 
             */
            return this.Ok();
        }
        #endregion

        #region Login

        /// <summary>
        /// Login by OIDC
        /// </summary>
        [HttpGet("Login")]
        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                OidUserInfo userInfo = new OidUserInfo
                {
                    UserName = this.HttpContext.User.Claims.Where(x => x.Type.Equals("sub")).FirstOrDefault().Value,
                    IdToken = await this.HttpContext.GetTokenAsync("id_token"),
                    AccessToken = await this.HttpContext.GetTokenAsync("access_token"),
                    RefreshToken = await this.HttpContext.GetTokenAsync("refresh_token"),
                    ExpiresAt = DateTimeOffset.Parse(await this.HttpContext.GetTokenAsync("expires_at"))
                };

                return await Task.FromResult(this.View(userInfo));
            }
            else
            {
                return this.Unauthorized();
            }
        }
        #endregion
    }
}
