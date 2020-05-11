using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Auth.Models;
using AspNetCore.IdentityServer4.Core.Models;
using AspNetCore.IdentityServer4.Service.Ldap;
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
    [Route("api/[controller]")]
    [ApiController]
    public class LdapUserController : ControllerBase
    {
        private readonly LdapUserManager ldapUserMgr = null;

        public LdapUserController(LdapUserManager ldapUserMgr)
        {
            this.ldapUserMgr = ldapUserMgr;
        }

        [HttpPost]
        public async Task<IActionResult> Create(LdapUserEntry entry)
        {
            var ldapUser = new OpenLdapUserEntry(
                entry.UserName, entry.Password, entry.Email, entry.DisplayName,entry.FirstName, entry.SecondName);
            if (await this.ldapUserMgr.CreateAsync(ldapUser))
                return this.StatusCode(StatusCodes.Status201Created);
            else
                return this.BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> Update(LdapUserEntry entry)
        {
            var ldapUser = new OpenLdapUserEntry(
                entry.UserName, entry.Password, entry.Email, entry.DisplayName, entry.FirstName, entry.SecondName);
            if (await this.ldapUserMgr.UpdateAsync(ldapUser))
                return this.Ok();
            else
                return this.BadRequest();
        }

        [HttpPut("ResetPwd")]
        public async Task<IActionResult> ResetPwd(LdapUserEntry entry)
        {
            if (await this.ldapUserMgr.ResetPwdAsync(entry.UserName, entry.Password))
                return this.Ok();
            else
                return this.BadRequest();
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
