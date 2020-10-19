using System;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Auth.Models;
using AspNetCore.IdentityServer4.Auth.Models.ViewModels;
using AspNetCore.IdentityServer4.Auth.Utils.Extensions;
using IdentityServer.LdapExtension.UserModel;
using IdentityServer.LdapExtension.UserStore;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.Auth.Areas.Account.Controllers
{
    /// <summary>
    /// Account controller
    /// </summary>
    [Route("[controller]")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger = null;
        private readonly IEventService events = null;
        private readonly IIdentityServerInteractionService interaction = null;
        private readonly IAuthenticationSchemeProvider schemeProvider = null;
        private readonly ILdapUserStore userStore = null;
        private readonly IClientStore clientStore = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="events">Event service</param>
        /// <param name="interaction">Idsrc interaction service</param>
        /// <param name="schemeProvider">Authentication scheme provider</param>
        /// <param name="userStore">User store</param>
        /// <param name="clientStore">Client store</param>
        public AccountController(
            ILogger<AccountController> logger,
            IEventService events,
            IIdentityServerInteractionService interaction,
            IAuthenticationSchemeProvider schemeProvider,
            ILdapUserStore userStore,
            IClientStore clientStore)
        {
            this.logger = logger;
            this.events = events;
            this.interaction = interaction;
            this.schemeProvider = schemeProvider;
            this.userStore = userStore;
            this.clientStore = clientStore;
        }
        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        [Route("Login")]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            LoginViewModel vm = await this.buildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { provider = vm.ExternalLoginScheme, returnUrl });
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [Route("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // check if we are in the context of an authorization request
            var context = await this.interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await this.interaction.GrantConsentAsync(context, ConsentResponse.Denied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (await this.clientStore.IsPkceClientAsync(context.ClientId))
                    {
                        // if the client is PKCE then we assume it's native, so this change in how to
                        // return the response is for better UX for the end user.
                        return View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });

                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        //// return this.LoadingPage("Redirect", model.ReturnUrl);
                    }

                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }

            if (ModelState.IsValid)
            {
                // validate username/password against Ldap
                var user = this.userStore.ValidateCredentials(model.Username, model.Password);

                if (user != default(IAppUser))
                {
                    // Raise UserLoginSuccessEvent
                    await this.events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username));

                    // only set explicit expiration here if user chooses "remember me". 
                    // otherwise we rely upon expiration configured in cookie middleware.
                    AuthenticationProperties props = null;
                    if (AccountOptions.AllowRememberLogin && model.RememberLogin)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                        };
                    };

                    // Response with authentication cookie
                    await this.HttpContext.SignInAsync(user.SubjectId, user.Username, props);

                    if (context != null)
                    {
                        if (await this.clientStore.IsPkceClientAsync(context?.ClientId))
                        {
                            // if the client is PKCE then we assume it's native, so this change in how to
                            // return the response is for better UX for the end user.
                            return View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });

                            // The client is native, so this change in how to
                            // return the response is for better UX for the end user.
                            ////return this.LoadingPage("Redirect", model.ReturnUrl);
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(model.ReturnUrl);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("Invalid return URL");
                    }
                }
                else
                {
                    await this.events.RaiseAsync(new UserLoginFailureEvent(model.Username, "Invalid credentials", clientId: context?.ClientId));
                    ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
                }
            }

            // something went wrong, show form with error
            var vm = await this.buildLoginViewModelAsync(model);
            return View(vm);
        }

        private async Task<LoginViewModel> buildLoginViewModelAsync(string returnUrl)
        {
            var context = await this.interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await this.schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var isLocal = context.IdP == IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = isLocal,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!isLocal)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

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

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
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

        private async Task<LoginViewModel> buildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await this.buildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }
    }
}
