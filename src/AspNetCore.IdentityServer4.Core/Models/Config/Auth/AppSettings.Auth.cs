namespace AspNetCore.IdentityServer4.Core.Models.Config.Auth
{
    /// <summary>
    /// AppSettings
    /// </summary>
    public class AppSettings : IAppSettings
    {
        /// <summary>
        /// LDAP options
        /// </summary>
        public LdapServerOptions LdapServer { get; set; }

        /// <summary>
        /// Host options
        /// </summary>
        public HostOptions Host { get; set; }

        /// <summary>
        /// Global options
        /// </summary>
        public GlobalOptions Global { get; set; }
    }
}
