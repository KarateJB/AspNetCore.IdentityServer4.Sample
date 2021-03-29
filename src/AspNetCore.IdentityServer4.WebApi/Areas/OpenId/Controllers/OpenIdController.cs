using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.WebApi.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.WebApi.Areas.Auth.Controllers
{
    /// <summary>
    /// Open ID controller
    /// </summary>
    [Route("[controller]")]
    // [Authorize] // DONOT specify the AuthenticationSchemes here
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

        #region Demo

        /// <summary>
        /// Demo page
        /// </summary>
        /// <remarks>A Demo page for authencated user.</remarks>
        [Authorize] // DONOT specify the AuthenticationSchemes here
        [HttpGet("Demo")]
        public async Task<IActionResult> Demo()
        {
            ViewBag.IsAuthenticated = this.HttpContext.User.Identity.IsAuthenticated;
            ViewBag.UserName = ViewBag.IsAuthenticated ? this.HttpContext.User.Claims.Where(x => x.Type.Equals("sub")).FirstOrDefault().Value : "Anonoymous";
            return this.View();
        }
        #endregion

        #region Login

        /// <summary>
        /// Login by OIDC
        /// </summary>
        [Authorize] // DONOT specify the AuthenticationSchemes here
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

        #region Logout

        [Authorize] // DONOT specify the AuthenticationSchemes here
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            this.Response.Cookies.Delete("idsrv");
            this.Response.Cookies.Delete("idsrv.session");
            // Delete local authentication cookie
            await HttpContext.SignOutAsync();
            // Clear the existing external cookie to ensure a clean login process
            ////return this.SignOut("Cookies", "oidc");
            // Clear the existing external cookie to ensure a clean login process
            ////await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return this.RedirectToAction(actionName: "Demo");
        }
        #endregion

        [HttpGet("Login/JS")]
        public async Task<IActionResult> LoginByJs()
        {
            return await Task.FromResult(this.View(new OidUserInfo()));
        }
    }
}