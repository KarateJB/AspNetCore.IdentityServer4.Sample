using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core.Models;
using AspNetCore.IdentityServer4.Core.Models.Config;
using AspNetCore.IdentityServer4.Core.Models.Config.Auth;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;

namespace AspNetCore.IdentityServer4.Service.Ldap
{
    public class LdapService
    {
        private readonly AppSettings appSettings = null;
        private readonly LdapServerOptions ldapServer = null;
        private bool isOpenLdap = false;

        public LdapService(IOptions<AppSettings> configuration)
        {
            this.appSettings = configuration.Value;
            this.ldapServer = this.appSettings?.LdapServer;
        }

        /// <summary>
        /// Find user by name
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>LdapEntry object</returns>
        public async Task<LdapEntry> FindUserAsync(string userName)
        {
            Func<LdapConnection, LdapEntry> action = (ldapConn) =>
            {
                // Set search filter
                var searchFilter = $"(cn={userName})";

                // Get search result
                LdapSearchResults searchResults = ldapConn.Search(
                    this.ldapServer.SearchBase,
                    scope: LdapConnection.SCOPE_SUB,
                    filter: searchFilter,
                    attrs: new string[] { "cn", "sAMAccountName" },
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
        public async Task<bool> AddUserAsync(OpenLdapUserEntry entry)
        {
            Func<LdapConnection, bool> action = (ldapConn) =>
            {
                if (this.FindUserAsync(entry.Uid).Result == null)
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
        /// Reset user's password
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="newPwd">New password</param>
        /// <returns>True(Success)/False(Fail)</returns>
        public async Task<bool> ResetPwdAsync(string userName, string newPwd)
        {
            Func<LdapConnection, bool> action = (ldapConn) =>
            {
                var entry = this.FindUserAsync(userName).Result;
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
        /// Get AttributeSet for OpenLDAP
        /// </summary>
        /// <param name="entry">OpenLdapUserEntry object</param>
        /// <returns>LdapAttributeSet</returns>
        private async Task<LdapAttributeSet> getAttrSetForOpenLdapAsync(OpenLdapUserEntry entry)
        {
            return await Task.FromResult(new LdapAttributeSet()
            {
                new LdapAttribute("objectClass", "inetOrgPerson"),
                new LdapAttribute("displayName", entry.DisplayName),
                new LdapAttribute("uid", entry.Uid),
                new LdapAttribute("sn", entry.SecondName),
                new LdapAttribute("mail", entry.Email),
                new LdapAttribute("userPassword", entry.Pwd),
                new LdapAttribute("givenName", entry.FirstName), // optional
            });
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
