using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Auth.Models;
using AspNetCore.IdentityServer4.Auth.Models.ViewModels;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.Auth.Controllers.Account
{
    /// <summary>
    /// Account controller
    /// </summary>
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger = null;
        private readonly IIdentityServerInteractionService interaction = null;
        private readonly IAuthenticationSchemeProvider schemeProvider = null;
        private readonly IClientStore clientStore;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="interaction">Idsrc interaction service</param>
        /// <param name="schemeProvider">Authentication scheme provider</param>
        /// <param name="clientStore">Client store</param>
        public AccountController(
            ILogger<AccountController> logger,
            IIdentityServerInteractionService interaction,
            IAuthenticationSchemeProvider schemeProvider,
            IClientStore clientStore)
        {
            this.logger = logger;
            this.interaction = interaction;
            this.schemeProvider = schemeProvider;
            this.clientStore = clientStore;
        }
        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            LoginViewModel vm = await this.buildLoginViewModelAsync(returnUrl);

            ////if (vm.IsExternalLoginOnly)
            ////{
            ////    // we only have one option for logging in and it's an external provider
            ////    return RedirectToAction("Challenge", "External", new { provider = vm.ExternalLoginScheme, returnUrl });
            ////}

            return View(vm);
        }

        private async Task<LoginViewModel> buildLoginViewModelAsync(string returnUrl)
        {
            var context = await this.interaction.GetAuthorizationContextAsync(returnUrl);
            ////if (context?.IdP != null)
            ////{
            ////    var isLocal = context.IdP == IdentityServerConstants.LocalIdentityProvider;

            ////    // this is meant to short circuit the UI and only trigger the one external IdP
            ////    var vm = new LoginViewModel
            ////    {
            ////        EnableLocalLogin = isLocal,
            ////        ReturnUrl = returnUrl,
            ////        Username = context?.LoginHint,
            ////    };

            ////    if (!isLocal)
            ////    {
            ////        vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
            ////    }

            ////    return vm;
            ////}

            var schemes = await this.schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await this.clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    ////if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    ////{
                    ////    providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    ////}
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }
    }
}
