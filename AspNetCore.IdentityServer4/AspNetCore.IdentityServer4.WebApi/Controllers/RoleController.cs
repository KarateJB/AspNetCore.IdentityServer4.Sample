using System;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.WebApi.Models;
using AspNetCore.IdentityServer4.WebApi.Utils;
using JB.Infra.Service.Redis;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.IdentityServer4.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ICacheService cache = null;
        private readonly RedisKeyFactory cacheKeys = null;

        public RoleController(
            ICacheService cache,
            RedisKeyFactory cacheKeys)
        {
            this.cache = cache;
            this.cacheKeys = cacheKeys;
        }

        [HttpGet("Get/{userName}")]
        public async Task<UserRole> Get([FromRoute] string userName)
        {
            var cacheKey = this.cacheKeys.GetKeyRoles(userName);
            (UserRole userRole, bool isOK) = await this.cache.GetCacheAsync<UserRole>(cacheKey);

            if (!isOK)
            {
                this.HttpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
            }

            return isOK ? userRole : null;
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Create([FromBody] UserRole userRole)
        {
            var cacheKey = this.cacheKeys.GetKeyRoles(userRole.Username);
            await this.cache.SaveCacheAsync<UserRole>(cacheKey, userRole);
            return this.Ok();
        }

        [HttpPost("Remove/{userName}")]
        public async Task<ActionResult> Remove([FromRoute] string userName)
        {
            var cacheKey = this.cacheKeys.GetKeyRoles(userName);
            await this.cache.ClearCacheAsync(cacheKey);
            return this.Ok();
        }
    }
}
