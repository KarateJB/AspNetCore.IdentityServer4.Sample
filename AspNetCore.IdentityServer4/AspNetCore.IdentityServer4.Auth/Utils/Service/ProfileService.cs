using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace AspNetCore.IdentityServer4.Auth.Utils.Service
{
    /// <summary>
    /// Profile service
    /// </summary>
    public class ProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {

            //sub is your userId.
            var subClaim = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub");

            if (!string.IsNullOrEmpty(subClaim?.Value))
            {
                
                context.IssuedClaims = this.getClaims(subClaim.Value);

                //get the actual user object from the database
                //var user = await _userService.GetUserAsync(long.Parse(userId.Value));

                // issue the claims for the user
                //if (user != null)
                //{
                //    var claims = this.GetClaims(user);

                //    //add the claims
                //    context.IssuedClaims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                //}
            }

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            // Find user by context.Subject.GetSubjectId()
            //var user = Users.FindBySubjectId(context.Subject.GetSubjectId());
            //context.IsActive = user?.IsActive == true;

            context.IsActive = true;
            return Task.CompletedTask;
        }

        private List<Claim> getClaims(string userId)
        {
            #region Method 1.Add extra const roles
            var claims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Role, "admin"),
                    new Claim(JwtClaimTypes.Role, "user")
                };
            #endregion

            #region Method 2. Add extra roles from redis

            #endregion

            return claims;
        }
    }
}
