using System.ComponentModel.DataAnnotations;

namespace AspNetCore.IdentityServer4.Core.Models
{
    /// <summary>
    /// User entry (OpenLDAP)
    /// </summary>
    public class OpenLdapUserEntry
    {
        [Required]
        public string Uid { get; set; }

        [Required]
        public string Pwd { get; set; }

        [Required]
        public string Email { get; set; }

        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        [Required]
        public string SecondName{ get; set; }

        public OpenLdapUserEntry(
            string dn, string uid, string pwd, string email, string displayName="", string firstName = "",  string secondName = "")
        {
            this.Uid = uid.ToLower();
            this.Pwd = pwd;
            this.Email = email;
            this.DisplayName = displayName ?? this.Uid;
            this.FirstName = firstName;
            this.SecondName = secondName ?? this.Uid;
        }
    }
}
