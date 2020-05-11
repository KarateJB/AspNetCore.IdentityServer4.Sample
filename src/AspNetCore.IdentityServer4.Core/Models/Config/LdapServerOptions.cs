namespace AspNetCore.IdentityServer4.Core.Models.Config
{
    /// <summary>
    /// LDAP server's options
    /// </summary>
    public class LdapServerOptions
    {
        /// <summary>
        /// LDAP Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// LDAP port
        /// </summary>
        public int Port { get; set; } = 389;

        /// <summary>
        /// Is enable SSL connection
        /// </summary>
        public bool Ssl { get; set; } = false;

        /// <summary>
        /// Bind DN
        /// </summary>
        public string BindDn { get; set; }

        /// <summary>
        /// Bind credential
        /// </summary>
        public string BindCredentials { get; set; }

        /// <summary>
        /// Search base
        /// </summary>
        public string SearchBase { get; set; }

        /// <summary>
        /// Search filter
        /// </summary>
        public string SearchFilter { get; set; }
    }
}
