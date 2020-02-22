using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Auth.Models;
using AspNetCore.IdentityServer4.WebApi.Models.AuthorizationRequirement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.WebApi.Handlers
{
    /// <summary>
    /// User name Authorization handler
    /// </summary>
    public class UserNameAuthHandler : AuthorizationHandler<UserNameRequirement>
    {
        private readonly ILogger logger = null;

        public UserNameAuthHandler(ILogger<UserNameAuthHandler> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Handle requirement
        /// </summary>
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UserNameRequirement requirement)
        {
            string validateMsgOK = $"Validate OK with {nameof(UserNameAuthHandler)}";
            string validateMsgNG = $"Validate NG with {nameof(UserNameAuthHandler)}";

            this.logger.LogDebug($"Start validating user claim with {nameof(UserNameAuthHandler)}");

            ClaimsPrincipal userClaim = context.User;
            // Verify the result of last Authorization handler
            if (userClaim.Identity == null || !userClaim.Identities.Any(i => i.IsAuthenticated))
            {
                return;
            }

            var username = userClaim.Identity.Name;

            // Verify User name
            if (!username.Equals(requirement.UserName))
            {
                this.logger.LogDebug(validateMsgNG);
                return;
            }

            this.logger.LogDebug(validateMsgOK);
        }
    }
}
