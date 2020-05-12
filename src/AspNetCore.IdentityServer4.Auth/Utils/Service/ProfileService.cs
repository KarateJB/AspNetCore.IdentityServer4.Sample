using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core.Models;
using AspNetCore.IdentityServer4.Core.Utils.Factory;
using AspNetCore.IdentityServer4.Service.Cache;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace AspNetCore.IdentityServer4.Auth.Utils.Service
{
    /// <summary>
    /// Profile service
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly ICacheService cache = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProfileService(ICacheService cache)
        {
            this.cache = cache;
        }

        /// <summary>
        /// Get profile
        /// </summary>
        /// <param name="context">ProfileDataRequestContext</param>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {

            //sub is your userId.
            var subClaim = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub");


            // Custom claims
            if (!string.IsNullOrEmpty(subClaim?.Value))
            {
                context.IssuedClaims = await this.getClaims(subClaim.Value);
            }

            // Add more claims, such as Email (See https://github.com/IdentityServer/IdentityServer4/issues/678)
            var emailClaim = context.Subject.Claims.FirstOrDefault(c => c.Type.Equals(IdentityModel.JwtClaimTypes.Email));
            if (emailClaim != null)
                context.IssuedClaims.Add(emailClaim);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Is active
        /// </summary>
        /// <param name="context">IsActiveContext</param>
        public async Task IsActiveAsync(IsActiveContext context)
        {
            // Find user by context.Subject.GetSubjectId()
            //var user = Users.FindBySubjectId(context.Subject.GetSubjectId());
            //context.IsActive = user?.IsActive == true;

            context.IsActive = true;
            await Task.CompletedTask;
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
            var cacheKey = CacheKeyFactory.UserProfile(userName);
            (UserProfile user, bool isOK) = await this.cache.GetCacheAsync<UserProfile>(cacheKey);

            if (isOK)
            {
                // Role claim
                user.Roles.Split(',').Select(x => new Claim(ClaimTypes.Role, x.Trim())).ToList().ForEach(claim => claims.Add(claim));

                // Department claim
                claims.Add(new Claim(CustomClaimTypes.Department, user.Department));
            }
            #endregion

            return claims;
        }
    }
}
