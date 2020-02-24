using Microsoft.AspNetCore.Authorization;

namespace AspNetCore.IdentityServer4.WebApi.Models.AuthorizationRequirement
{
    /// <summary>
    /// Custom Authorization Requirement for certain user name
    /// </summary>
    public class EmailDomainRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// The domain name
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="domain">Required domain</param>
        public EmailDomainRequirement(string domain)
        {
            this.Domain = domain;
        }
    }
}
