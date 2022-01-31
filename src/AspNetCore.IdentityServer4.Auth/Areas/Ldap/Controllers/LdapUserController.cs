using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Auth.Models;
using AspNetCore.IdentityServer4.Core.Models;
using AspNetCore.IdentityServer4.Core.Utils.Factory;
using AspNetCore.IdentityServer4.Service.Ldap;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.IdentityServer4.Auth.Areas.Ldap.Controllers
{
    /// <summary>
    /// LDAP User controller
    /// </summary>
    [Route(RouteFactory.ApiController)]
    [ApiController]
    public class LdapUserController : ControllerBase
    {
        private readonly LdapUserManager ldapUserMgr = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ldapUserMgr">LDAP User manager</param>
        public LdapUserController(LdapUserManager ldapUserMgr)
        {
            this.ldapUserMgr = ldapUserMgr;
        }

        /// <summary>
        /// Create a LDAP User
        /// </summary>
        /// <param name="entry">LdapUserEntry object</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> Create(LdapUserEntry entry)
        {
            var ldapUser = new OpenLdapUserEntry(
                entry.UserName, entry.Password, entry.Email, entry.DisplayName, entry.FirstName, entry.SecondName);
            if (await this.ldapUserMgr.CreateAsync(ldapUser))
                return this.StatusCode(StatusCodes.Status201Created);
            else
                return this.StatusCode(StatusCodes.Status409Conflict);
        }

        /// <summary>
        /// Update a LDAP User
        /// </summary>
        /// <param name="entry">LdapUserEntry object</param>
        /// <returns>IActionResult</returns>
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

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="entry">LdapUserEntry object</param>
        /// <returns>IActionResult</returns>
        [HttpPut("ResetPwd")]
        public async Task<IActionResult> ResetPwd(LdapUserEntry entry)
        {
            if (await this.ldapUserMgr.ResetPwdAsync(entry.UserName, entry.Password))
                return this.Ok();
            else
                return this.BadRequest();
        }

        /// <summary>
        /// Remove a LDAP user
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>IActionResult</returns>
        [HttpDelete("{userName}")]
        public async Task<IActionResult> Remove([FromRoute] string userName)
        {
            if (await this.ldapUserMgr.RemoveAsync(userName))
                return this.Ok();
            else
                return this.BadRequest();
        }
    }
}
