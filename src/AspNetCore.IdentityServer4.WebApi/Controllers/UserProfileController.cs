using System;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core;
using AspNetCore.IdentityServer4.Core.Models;
using AspNetCore.IdentityServer4.Service.Cache;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.IdentityServer4.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly ICacheService cache = null;

        public UserProfileController(ICacheService cache)
        {
            this.cache = cache;
        }

        [HttpGet("Get/{userName}")]
        public async Task<UserProfile> Get([FromRoute] string userName)
        {
            var cacheKey = CacheKeyFactory.UserProfile(userName);
            (UserProfile userRole, bool isOK) = await this.cache.GetCacheAsync<UserProfile>(cacheKey);

            if (!isOK)
            {
                this.HttpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
            }

            return isOK ? userRole : null;
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Create([FromBody] UserProfile user)
        {
            var cacheKey = CacheKeyFactory.UserProfile(user.Username);
            await this.cache.SaveCacheAsync<UserProfile>(cacheKey, user);
            return this.Ok();
        }

        [HttpPost("Remove/{userName}")]
        public async Task<ActionResult> Remove([FromRoute] string userName)
        {
            var cacheKey = CacheKeyFactory.UserProfile(userName);
            await this.cache.ClearCacheAsync(cacheKey);
            return this.Ok();
        }
    }
}
