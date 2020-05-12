using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Auth.Models;
using IdentityModel;
using IdentityServer.LdapExtension.UserModel;
using IdentityServer.LdapExtension.UserStore;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.IdentityServer4.Auth.Controllers
{
    /// <summary>
    /// LDAP controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class LdapController : ControllerBase
    {
        private readonly ILdapUserStore userStore = null;
        private readonly IEventService events = null;
        private readonly IdentityServerTools tools = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userStore">LDAP User Store</param>
        /// <param name="events">IEventService</param>
        /// <param name="tools">IdentityServerTools</param>
        public LdapController(
            ILdapUserStore userStore,
            IEventService events,
            IdentityServerTools tools)
        {
            this.userStore = userStore;
            this.events = events;
            this.tools = tools;
        }

        /// <summary>
        /// Sign in
        /// </summary>
        /// <param name="model">LdapUser object</param>
        /// <returns>IActionResult</returns>
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody]LdapUser model)
        {
            // validate username/password against Ldap
            var user = this.userStore.ValidateCredentials(model.Username, model.Password);

            if (user != default(IAppUser))
            {
                // Response with authentication cookie
                await this.HttpContext.SignInAsync(user.SubjectId, user.Username);

                // Get the Access token
                var accessToken = await this.tools.IssueJwtAsync(lifetime: 3600, claims: new Claim[] { new Claim(JwtClaimTypes.Audience, model.ApiResource) });

                // Save Access token to current session
                this.HttpContext.Session.SetString("AccessToken", accessToken);

                // Write the Access token to response
                await this.HttpContext.Response.WriteAsync(accessToken);

                // Raise UserLoginSuccessEvent
                await this.events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username));

                return this.Ok();
            }
            else
            {
                return this.Unauthorized();
            }
        }

        /// <summary>
        /// Validate LDAP user
        /// </summary>
        /// <param name="user">LdapUser object</param>
        /// <returns>IActionResult</returns>
        [HttpPost("Validate")]
        public async Task<IActionResult> Validate([FromBody]LdapUser user)
        {
            var isAuthorized = await this.ExecLdapAuthAsync(user.Username, user.Password);

            if (isAuthorized)
            {
                return this.Ok();
            }
            else
            {
                return this.Unauthorized();
            }
        }

        private async Task<bool> ExecLdapAuthAsync(string username, string password)
        {
            var host = "jb.com"; // Host
            var bindDN = "cn=admin,dc=example,dc=org";
            var bindPassword = "admin";
            var baseDC = "dc=example,dc=org";
            bool isAuthorized = false;

            try
            {
                isAuthorized = await Task.Run(() =>
                {
                    using (var connection = new Novell.Directory.Ldap.LdapConnection())
                    {
                        connection.Connect(host, Novell.Directory.Ldap.LdapConnection.DEFAULT_PORT);
                        connection.Bind(bindDN, bindPassword);

                        var searchFilter = $"(&(objectClass=person)(uid={username}))";
                        var entities = connection.Search(
                            baseDC,
                            Novell.Directory.Ldap.LdapConnection.SCOPE_SUB,
                            searchFilter,
                            new string[] { "uid", "cn", "mail" },
                            false);

                        string userDn = null;

                        while (entities.hasMore())
                        {
                            var entity = entities.next();
                            var account = entity.getAttribute("uid");
                            if (account != null && account.StringValue == username)
                            {
                                userDn = entity.DN;
                                break;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(userDn))
                        {
                            return false;
                        }

                        try
                        {
                            connection.Bind(userDn, password);
                            return connection.Bound;
                        }
                        catch (System.Exception)
                        {
                            return false;
                        }
                    }
                });

                return isAuthorized;
            }
            catch (Novell.Directory.Ldap.LdapException e)
            {
                throw e;
            }
        }
    }
}
