using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.WebApi.Models.AuthorizationRequirement;
using IdentityModel;
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

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="logger">Logger</param>
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

            ClaimsPrincipal userClaim = context.User;

            if (context.HasFailed || userClaim.Identity == null || !userClaim.Identities.Any(i => i.IsAuthenticated))
            {
                context.Fail();
                this.logger.LogWarning($"Skip validating with {nameof(UserNameAuthHandler)} cus the authorization process had been failed!");
                return;
            }

            var nameClaim = userClaim.Claims.FirstOrDefault(c => c.Type.Equals(JwtClaimTypes.Subject)); // Or ClaimTypes.NameIdentifier

            if (nameClaim != null)
            {
                // Verify name
                if (nameClaim.Value.Equals(requirement.UserName))
                {
                    this.logger.LogDebug(validateMsgOK);
                    context.Succeed(requirement);
                    return;
                }
            }

            this.logger.LogDebug(validateMsgNG);
            context.Fail();

            await Task.CompletedTask;
        }
    }
}
