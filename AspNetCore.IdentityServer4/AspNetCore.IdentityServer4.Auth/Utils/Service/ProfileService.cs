using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core;
using AspNetCore.IdentityServer4.Core.Models;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using JB.Infra.Service.Redis;

namespace AspNetCore.IdentityServer4.Auth.Utils.Service
{
    /// <summary>
    /// Profile service
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly ICacheService cache = null;
        private readonly CacheKeyFactory cacheKeys = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProfileService(
            ICacheService cache,
            CacheKeyFactory cacheKeys)
        {
            this.cache = cache;
            this.cacheKeys = cacheKeys;
        }
        
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {

            //sub is your userId.
            var subClaim = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub");

            if (!string.IsNullOrEmpty(subClaim?.Value))
            {
                context.IssuedClaims = await this.getClaims(subClaim.Value);
            }

            // return Task.CompletedTask;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            // Find user by context.Subject.GetSubjectId()
            //var user = Users.FindBySubjectId(context.Subject.GetSubjectId());
            //context.IsActive = user?.IsActive == true;

            context.IsActive = true;
            // return Task.CompletedTask;
        }

        private async Task<List<Claim>> getClaims(string userName)
        {
            var claims = new List<Claim>();

            #region Method 1.Add extra const roles
            //claims = new List<Claim>
            //    {
            //        new Claim(JwtClaimTypes.Role, "admin"),
            //        new Claim(JwtClaimTypes.Role, "user")
            //    };
            #endregion

            #region Method 2. Add extra roles from redis
            var cacheKey = this.cacheKeys.GetKeyRoles(userName);
            (UserProfile userRole, bool isOK) = await this.cache.GetCacheAsync<UserProfile>(cacheKey);

            if (isOK)
            {
                claims = userRole.Roles.Split(',').Select( x => new Claim(ClaimTypes.Role, x.Trim())).ToList();
            }
            #endregion

            return claims;
        }
    }
}
