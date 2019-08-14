﻿using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Auth.Models;
using IdentityServer.LdapExtension.UserModel;
using IdentityServer.LdapExtension.UserStore;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.IdentityServer4.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LdapController : ControllerBase
    {
        private readonly ILdapUserStore userStore = null;

        public LdapController(
            ILdapUserStore userStore)
        {
            this.userStore = userStore;
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody]LdapUser model)
        {
            // validate username/password against Ldap
            var user = this.userStore.ValidateCredentials(model.Username, model.Password);

            if (user != default(IAppUser))
            {
                // Response with authentication cookie
                await this.HttpContext.SignInAsync(user.SubjectId, user.Username);

                var token = await this.HttpContext.GetTokenAsync("access_token");

                return this.Ok();
            }
            else
            {
                return this.Unauthorized();
            }
        }

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
            var host = "jblin"; // Host
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
