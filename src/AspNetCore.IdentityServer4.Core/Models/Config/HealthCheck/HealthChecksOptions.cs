using System.Collections.Generic;

namespace AspNetCore.IdentityServer4.Core.Models.Config.HealthCheck
{
    /// <summary>
    /// HealthChecks options
    /// </summary>
    public class HealthChecksOptions
    {
        /// <summary>
        /// Service options
        /// </summary>
        public ServiceOptions Service { get; set; }

        /// <summary>
        /// Endpoints options
        /// </summary>
        public List<EndpointOptions> Endpoints { get; set; }

    }
}