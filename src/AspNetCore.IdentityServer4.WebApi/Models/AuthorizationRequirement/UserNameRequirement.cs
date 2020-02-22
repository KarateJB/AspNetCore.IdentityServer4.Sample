using Microsoft.AspNetCore.Authorization;

namespace AspNetCore.IdentityServer4.WebApi.Models.AuthorizationRequirement
{
    /// <summary>
    /// Custom Authorization Requirement for certain user name
    /// </summary>
    public class UserNameRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// User's name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="username"></param>
        public UserNameRequirement(string username)
        {
            this.UserName = username;
        }
    }
}
