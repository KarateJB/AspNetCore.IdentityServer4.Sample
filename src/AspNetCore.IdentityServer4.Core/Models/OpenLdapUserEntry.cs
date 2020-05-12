using System.ComponentModel.DataAnnotations;
using AspNetCore.IdentityServer4.Core.Utils.Attributes;

namespace AspNetCore.IdentityServer4.Core.Models
{
    /// <summary>
    /// User entry (OpenLDAP)
    /// </summary>
    public class OpenLdapUserEntry
    {
        [Required]
        [LdapAttr("uid", true)]
        public string Uid { get; set; }

        [LdapAttr("userPassword", true)]
        public string Pwd { get; set; }

        [LdapAttr("mail", true)]
        public string Email { get; set; }

        [LdapAttr("displayName")]
        public string DisplayName { get; set; }

        [LdapAttr("givenName")]
        public string FirstName { get; set; }

        [LdapAttr("sn", true)]
        public string SecondName{ get; set; }

        public OpenLdapUserEntry(
            string uid, string pwd, string email, string displayName="", string firstName = "",  string secondName = "")
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
