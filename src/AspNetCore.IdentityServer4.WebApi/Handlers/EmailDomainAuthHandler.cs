using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Auth.Models;
using AspNetCore.IdentityServer4.WebApi.Models.AuthorizationRequirement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.WebApi.Handlers
{
    /// <summary>
    /// Email domain Authorization handler
    /// </summary>
    public class EmailDomainAuthHandler : AuthorizationHandler<EmailDomainRequirement>
    {
        private readonly ILogger logger = null;

        public EmailDomainAuthHandler(ILogger<EmailDomainAuthHandler> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Handle requirement
        /// </summary>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EmailDomainRequirement requirement)
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

            var userEmailClaim = userClaim.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email));

            if (userEmailClaim != null)
            {
                // Get domain name from email
                var address = new MailAddress(userEmailClaim.Value);
                string userDomain = address.Host; // e.q. google.com

                // Verify domain
                if (userDomain.Equals(requirement.Domain))
                {
                    this.logger.LogDebug(validateMsgOK);
                    return;
                }
            }
            else
            {
                this.logger.LogDebug(validateMsgNG);
                return;

            }
        }
    }
}
