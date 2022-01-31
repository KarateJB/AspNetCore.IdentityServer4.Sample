namespace AspNetCore.IdentityServer4.Core.Models.Config.HealthCheck
{
    /// <summary>
    /// Endpoints options
    /// </summary>
    public class EndpointOptions
    {
        /// <summary>
        /// The name of endpoint
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }
    }
}