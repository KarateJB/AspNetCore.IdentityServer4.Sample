namespace AspNetCore.IdentityServer4.Auth.Models
{
    /// <summary>
    /// LDAP User
    /// </summary>
    public class LdapUser
    {
         /// <summary>
        /// User name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// API resource name of Scope
        /// </summary>
        public string ApiResource { get; set; }
    }
}