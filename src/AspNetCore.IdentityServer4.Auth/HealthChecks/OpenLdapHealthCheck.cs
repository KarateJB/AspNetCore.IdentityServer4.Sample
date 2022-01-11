using System.Threading;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core.Models.Config.Auth;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;

namespace AspNetCore.IdentityServer4.Auth.HealthChecks
{
    /// <summary>
    /// OpenLDAP Health check
    /// </summary>
    public class OpenLdapHealthCheck : IHealthCheck
    {
        private readonly AppSettings appSettings;
        private readonly LdapServerOptions ldapServer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public OpenLdapHealthCheck(IOptions<AppSettings> configuration)
        {
            this.appSettings = configuration.Value;
            this.ldapServer = this.appSettings?.LdapServer;
        }

        /// <summary>
        /// Check health
        /// </summary>
        /// <param name="context">HealthCheckContext</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>HealthCheckResult</returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var ldapConn = new LdapConnection())
            {
                // Set LDAP connection
                ldapConn.SecureSocketLayer = this.ldapServer.Ssl;
                ldapConn.Connect(this.ldapServer.Url, this.ldapServer.Port);
                ldapConn.Bind(dn: this.ldapServer.BindDn, passwd: this.ldapServer.BindCredentials);
                return ldapConn.Connected ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
            }
        }
    }
}
