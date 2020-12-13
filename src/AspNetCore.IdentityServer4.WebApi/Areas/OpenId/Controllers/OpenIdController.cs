using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
                string accessToken = await HttpContext.GetTokenAsync("access_token");
                string idToken = await HttpContext.GetTokenAsync("id_token");
                string refreshToken = await HttpContext.GetTokenAsync("refresh_token");
                var expiresAt = DateTimeOffset.Parse(await HttpContext.GetTokenAsync("expires_at"));
                return await Task.FromResult(this.View());
            }
            else
            {
                return this.Unauthorized();
            }
        }
        #endregion
    }
}
