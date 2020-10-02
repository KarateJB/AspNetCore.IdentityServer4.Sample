using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.IdentityServer4.Auth.Controllers.Account
{
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            throw new NotImplementedException();
            // build a model so we know what to show on the login page
            //// var vm = await BuildLoginViewModelAsync(returnUrl);

            ////if (vm.IsExternalLoginOnly)
            ////{
            ////    // we only have one option for logging in and it's an external provider
            ////    return RedirectToAction("Challenge", "External", new { provider = vm.ExternalLoginScheme, returnUrl });
            ////}

            ////return View(vm);
        }
    }
}
