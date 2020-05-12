using System.ComponentModel.DataAnnotations;

namespace AspNetCore.IdentityServer4.Auth.Models
{
    /// <summary>
    /// User entry
    /// </summary>
    public class LdapUserEntry
    {
        [Required]
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }
        
        public string FirstName { get; set; }
        
        public string SecondName{ get; set; }
    }
}
