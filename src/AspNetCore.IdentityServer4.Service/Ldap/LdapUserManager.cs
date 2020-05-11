using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core.Models;
using AspNetCore.IdentityServer4.Core.Models.Config;
using AspNetCore.IdentityServer4.Core.Models.Config.Auth;
using AspNetCore.IdentityServer4.Core.Utils.Attributes;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;

namespace AspNetCore.IdentityServer4.Service.Ldap
{
    /// <summary>
    /// LDAP User manager
    /// </summary>
    public class LdapUserManager
    {
        private readonly AppSettings appSettings = null;
        private readonly LdapServerOptions ldapServer = null;

        public LdapUserManager(IOptions<AppSettings> configuration)
        {
            this.appSettings = configuration.Value;
            this.ldapServer = this.appSettings?.LdapServer;
        }

        /// <summary>
        /// Find user by name
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>LdapEntry object</returns>
        /// <see cref="https://www.novell.com/documentation/developer/ldapcsharp/?page=/documentation/developer/ldapcsharp/cnet/data/bow8dju.html"/>
        public async Task<LdapEntry> FindAsync(string userName)
        {
            Func<LdapConnection, LdapEntry> action = (ldapConn) =>
            {
                // Set search filter
                var searchFilter = $"(cn={userName})";

                // Get search result
                LdapSearchResults searchResults = ldapConn.Search(
                    this.ldapServer.SearchBase,
                    scope: LdapConnection.SCOPE_SUB,
                    filter: searchFilter, // Search filter
                    attrs: new string[] { "cn", "displayName", "mail" }, // The attributes to retrieve
                    typesOnly: false);

                // Note in Novell.Directory.Ldap.NETStandard >=3.0.1, LdapSearchResults implement IEnumerable...
                // return searchResults.AsEnumerable().FirstOrDefault();
                while (searchResults.hasMore())
                {
                    var entry = searchResults.next();
                    return entry;
                }

                return default(LdapEntry);
            };

            return await this.ldapActionAsync(action);
        }

        /// <summary>
        /// Add a new LDAP user
        /// </summary>
        /// <param name="entry">OpenLdapUserEntry object</param>
        /// <returns>True(Success)/False(Fail)</returns>
        public async Task<bool> CreateAsync(OpenLdapUserEntry entry)
        {
            Func<LdapConnection, bool> action = (ldapConn) =>
            {
                if (this.FindAsync(entry.Uid).Result == null)
                {
                    var attributeSet = this.getAttrSetForOpenLdapAsync(entry).Result;
                    string userDn = this.getUserDefaultDnAsync(entry.Uid).Result;
                    ldapConn.Add(new LdapEntry(userDn, attributeSet));
                    return true;
                }
                else
                    return false;
            };

            return await this.ldapActionAsync(action);
        }

        /// <summary>
        /// Add a new LDAP user
        /// </summary>
        /// <param name="entry">OpenLdapUserEntry object</param>
        /// <returns>True(Success)/False(Fail)</returns>
        public async Task<bool> UpdateAsync(OpenLdapUserEntry entry)
        {
            Func<LdapConnection, bool> action = (ldapConn) =>
            {
                var existEntry = this.FindAsync(entry.Uid).Result;
                if (existEntry != null)
                {
                    var modifiedAttributes = new ArrayList();

                    // Iterate all properties and add to modifiedAttributes
                    PropertyInfo[] props = typeof(OpenLdapUserEntry).GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        var ldapAttr = Attribute.GetCustomAttributes(prop).FirstOrDefault( a => a.GetType().Equals(typeof(LdapAttrAttribute))) as LdapAttrAttribute;

                        if (ldapAttr != null)
                        {
                            var name = ldapAttr.Name;
                            var value = prop.GetValue(entry, null)?.ToString();

                            if (!string.IsNullOrEmpty(value))
                                modifiedAttributes.Add(new LdapModification(LdapModification.REPLACE, new LdapAttribute(name, value)));
                        }
                    }

                    var ldapModification = new LdapModification[modifiedAttributes.Count];
                    ldapModification = (LdapModification[])modifiedAttributes.ToArray(typeof(LdapModification));

                    ldapConn.Modify(existEntry.DN, ldapModification);
                    return true;
                }
                else
                    return false;
            };

            return await this.ldapActionAsync(action);
        }

        /// <summary>
        /// Reset user's password
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="newPwd">New password</param>
        /// <returns>True(Success)/False(Fail)</returns>
        public async Task<bool> ResetPwdAsync(string userName, string newPwd)
        {
            Func<LdapConnection, bool> action = (ldapConn) =>
            {
                var entry = this.FindAsync(userName).Result;
                if (entry != null)
                {
                    var modifiedAttributes = new ArrayList
                    {
                        new LdapModification(
                            LdapModification.REPLACE, new LdapAttribute("userPassword", newPwd))
                    };
    
                    var ldapModification = new LdapModification[modifiedAttributes.Count];
                    ldapModification = (LdapModification[])modifiedAttributes.ToArray(typeof(LdapModification));

                    ldapConn.Modify(entry.DN, ldapModification);
                    return true;
                }
                else
                    return false;
            };

            return await this.ldapActionAsync(action);
        }

        /// <summary>
        /// Remove a user
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>True(Success)/False(Fail)</returns>
        public async Task<bool> RemoveAsync(string userName)
        {
            Func<LdapConnection, bool> action = (ldapConn) =>
            {
                var existEntry = this.FindAsync(userName).Result;
                if (existEntry != null)
                {
                    ldapConn.Delete(existEntry.DN);
                    return true;
                }
                else
                    return false;
            };

            return await this.ldapActionAsync(action);
        }

        /// <summary>
        /// Get AttributeSet for OpenLDAP
        /// </summary>
        /// <param name="entry">OpenLdapUserEntry object</param>
        /// <returns>LdapAttributeSet</returns>
        private async Task<LdapAttributeSet> getAttrSetForOpenLdapAsync(OpenLdapUserEntry entry)
        {
            #region Required attributes

            var attrSet = new LdapAttributeSet()
            {
                new LdapAttribute("objectClass", "inetOrgPerson"),
                new LdapAttribute("displayName", entry.DisplayName),
                new LdapAttribute("uid", entry.Uid),
                new LdapAttribute("sn", entry.SecondName),
                new LdapAttribute("mail", entry.Email),
                new LdapAttribute("userPassword", entry.Pwd)
            };
            #endregion

            #region Optional attributes
            // Notice that the value cannot be null!

            if(!string.IsNullOrEmpty(entry.FirstName))
                attrSet.Add(new LdapAttribute("givenName", entry.FirstName));
            #endregion

            return await Task.FromResult(attrSet);
        }

        /// <summary>
        /// Get AttributeSet for Active Directory
        /// </summary>
        private async Task<LdapAttributeSet> getAttrSetForAdAsync(string sAMAccountName, string pwd, string email, string displayName = "")
        {
            if (string.IsNullOrEmpty(sAMAccountName) || string.IsNullOrEmpty(pwd) || string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("sAMAccountName, pwd and email are required");
            }

            // var unicodePwd = Encoding.Unicode.GetBytes($@"""{pwd}"""); // See issue: https://github.com/dsbenghe/Novell.Directory.Ldap.NETStandard/issues/31 

            var unicodePwd = SupportClass.ToSByteArray(Encoding.Unicode.GetBytes($@"""{pwd}"""));
            sAMAccountName = sAMAccountName.ToLower();
            displayName = displayName ?? sAMAccountName;

            return await Task.FromResult(new LdapAttributeSet()
            {
                new LdapAttribute("objectClass", new string[] { "user", "organizationalPerson", "person", "top" }),
                new LdapAttribute("sAMAccountName", sAMAccountName),
                new LdapAttribute("displayName", displayName),
                new LdapAttribute("mail", email),
                new LdapAttribute("userAccountControl", "66048"), // 66048 - Enabled, password never expires
                new LdapAttribute("unicodePwd", unicodePwd)
            });
        }

        private async Task<T> ldapActionAsync<T>(Func<LdapConnection, T> action)
        {
            using (var ldapConn = new LdapConnection())
            {
                // Set LDAP connection
                ldapConn.SecureSocketLayer = this.ldapServer.Ssl;
                ldapConn.Connect(this.ldapServer.Url, this.ldapServer.Port);
                ldapConn.Bind(dn: this.ldapServer.BindDn, passwd: this.ldapServer.BindCredentials);

                return await Task.FromResult(action(ldapConn));
            }
        }

        private async Task<string> getUserDefaultDnAsync(string userName)
        {
            return await Task.FromResult($"cn={userName},dc=example,dc=org");
        }
    }
}
