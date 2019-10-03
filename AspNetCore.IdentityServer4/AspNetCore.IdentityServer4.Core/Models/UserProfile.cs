namespace AspNetCore.IdentityServer4.Core.Models
{
    /// <summary>
    /// User profile for caching in Redis
    /// </summary>
    public class UserProfile
    {
        /// <summary>
        /// User name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// User roles
        /// </summary>
        public string Roles { get; set; }

        /// <summary>
        /// Department
        /// </summary>
        public string Department { get; set; }
    }
}
