using System;

namespace AspNetCore.IdentityServer4.Core.Utils.Attributes
{
    /// <summary>
    /// LDAP attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class LdapAttrAttribute : System.Attribute
    {
        /// <summary>
        /// The name of LDAP attribute
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is the LDAP attribute required
        /// </summary>
        public bool IsRequired { get; set; } = false;

        public LdapAttrAttribute(string name, bool isRequired = false)
        {
            this.Name = name;
            this.IsRequired = isRequired;
        }
    }
}
