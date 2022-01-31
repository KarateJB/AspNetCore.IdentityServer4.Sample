using System;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core.Models;
using AspNetCore.IdentityServer4.Core.Utils.Factory;
using AspNetCore.IdentityServer4.Service.Cache;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.IdentityServer4.WebApi.Areas.Profiles.Controllers
{
    /// <summary>
    /// User Profile controller
    /// </summary>
    [Route(RouteFactory.ApiController)]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly ICacheService cache = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cache">Cache service</param>
        public UserProfileController(ICacheService cache)
        {
            this.cache = cache;
        }

        /// <summary>
        /// Get user profile
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>User profile</returns>
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

        /// <summary>
        /// Create new user profile
        /// </summary>
        /// <param name="user">User profile</param>
        [HttpPost("Create")]
        public async Task<ActionResult> Create([FromBody] UserProfile user)
        {
            var cacheKey = CacheKeyFactory.UserProfile(user.Username);
            await this.cache.SaveCacheAsync<UserProfile>(cacheKey, user);
            return this.Ok();
        }

        /// <summary>
        /// Remove a user's user profile
        /// </summary>
        /// <param name="userName">User name</param>
        [HttpPost("Remove/{userName}")]
        public async Task<ActionResult> Remove([FromRoute] string userName)
        {
            var cacheKey = CacheKeyFactory.UserProfile(userName);
            await this.cache.ClearCacheAsync(cacheKey);
            return this.Ok();
        }
    }
}
