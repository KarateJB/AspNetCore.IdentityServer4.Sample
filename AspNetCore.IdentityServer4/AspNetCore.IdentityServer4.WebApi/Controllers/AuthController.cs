using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Auth.Models;
using AspNetCore.IdentityServer4.WebApi.Models;
using AspNetCore.IdentityServer4.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AspNetCore.IdentityServer4.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppSettings appSettings = null;
        private readonly IAuthService auth = null;

        public AuthController(
            IOptions<AppSettings> configuration,
            IAuthService authService)
        {
            this.appSettings = configuration.Value;
            this.auth = authService;
        }

        // GET api/values
        [HttpPost("SignIn")]
        [AllowAnonymous]
        public async Task<User> SignIn(LdapUser user)
        {
            User matchedUser = new User();
            var tokenResponse = await this.auth.SignInAsync(user.Username, user.Password);

            if (!tokenResponse.IsError)
            {
                // Authentication success
                matchedUser.AccessToken = tokenResponse.AccessToken;
                return matchedUser;
            }

            return matchedUser;
        }
    }
}
