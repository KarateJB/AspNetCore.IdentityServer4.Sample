namespace AspNetCore.IdentityServer4.Core.Models.Config.HealthCheck
{
    /// <summary>
    /// Service options
    /// </summary>
    public class ServiceOptions
    {
        /// <summary>
        /// Redis
        /// </summary>
        public HostPortOptions Redis { get; set; }

        /// <summary>
        /// Identity Server
        /// </summary>
        public HostPortOptions IdentityServer { get; set; }
    }
}