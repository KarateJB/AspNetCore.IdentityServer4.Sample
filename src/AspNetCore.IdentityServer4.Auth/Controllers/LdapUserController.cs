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
