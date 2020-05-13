using System.ComponentModel.DataAnnotations;

namespace AspNetCore.IdentityServer4.Auth.Models
{
    /// <summary>
    /// LDAP User entry
    /// </summary>
    public class LdapUserEntry
    {
        /// <summary>
        /// User name = cn = uid (OpenLDAP) = sAMAccountName (AD)
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// Last name
        /// </summary>
        public string SecondName{ get; set; }
    }
}
