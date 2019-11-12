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
        private readonly CacheKeyFactory cacheKeys = null;

        public UserProfileController(
            ICacheService cache,
            CacheKeyFactory cacheKeys)
        {
            this.cache = cache;
            this.cacheKeys = cacheKeys;
        }

        [HttpGet("Get/{userName}")]
        public async Task<UserProfile> Get([FromRoute] string userName)
        {
            var cacheKey = this.cacheKeys.UserProfile(userName);
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
            var cacheKey = this.cacheKeys.UserProfile(user.Username);
            await this.cache.SaveCacheAsync<UserProfile>(cacheKey, user);
            return this.Ok();
        }

        [HttpPost("Remove/{userName}")]
        public async Task<ActionResult> Remove([FromRoute] string userName)
        {
            var cacheKey = this.cacheKeys.UserProfile(userName);
            await this.cache.ClearCacheAsync(cacheKey);
            return this.Ok();
        }
    }
}
