using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Auth.Models.ViewModels;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.Auth.Controllers.Account
{
    /// <summary>
    /// Account controller
    /// </summary>
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> logger = null;
        private readonly IIdentityServerInteractionService interaction = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="interaction">Idsrc interaction service</param>
        public AccountController(
            ILogger<AccountController> logger,
            IIdentityServerInteractionService interaction)
        {
            this.logger = logger;
            this.interaction = interaction;
        }
        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            throw new NotImplementedException();

            // build a model so we know what to show on the login page
            LoginViewModel vm = await this.buildLoginViewModelAsync(returnUrl);

            ////if (vm.IsExternalLoginOnly)
            ////{
            ////    // we only have one option for logging in and it's an external provider
            ////    return RedirectToAction("Challenge", "External", new { provider = vm.ExternalLoginScheme, returnUrl });
            ////}

            ////return View(vm);
        }

        private async Task<LoginViewModel> buildLoginViewModelAsync(string returnUrl)
        {
            throw new NotImplementedException();
        }
    }
}
