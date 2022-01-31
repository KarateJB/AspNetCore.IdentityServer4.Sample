using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core.Models.Config.HealthCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore.IdentityServer4.HealthCheck.HealthChecks
{
    /// <summary>
    /// Identity Server Health check
    /// </summary>
    public class IdsrvHealthCheck : IHealthCheck
    {
        private readonly AppSettings appSettings;

        public IdsrvHealthCheck(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        /// <summary>
        /// Check health
        /// </summary>
        /// <param name="context">HealthCheckContext</param>
        /// <param name="cancellationToken">Cacellation token</param>
        /// <returns>Health check result</returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // Ignore certificate validation
            using var httpClient = new HttpClient(clientHandler);
            var idsrvOptions = this.appSettings.HealthChecks?.Service?.IdentityServer;
            httpClient.BaseAddress = new System.Uri($"https://{idsrvOptions.Host}:{idsrvOptions.Port}/");
            var response = await httpClient.GetAsync(".well-known/openid-configuration");
            return response.IsSuccessStatusCode ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }
    }
}
