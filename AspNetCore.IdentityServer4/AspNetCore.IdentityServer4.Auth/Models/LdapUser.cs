namespace AspNetCore.IdentityServer4.Auth.Models
{
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
    }
}