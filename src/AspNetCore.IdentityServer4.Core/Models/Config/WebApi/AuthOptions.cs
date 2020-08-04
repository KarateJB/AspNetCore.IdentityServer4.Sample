namespace AspNetCore.IdentityServer4.Core.Models.Config.WebApi
{
    /// <summary>
    /// Auth Options
    /// </summary>
    public class AuthOptions
    {
        /// <summary>
        /// Audience
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Client id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Frequency to refresh discovery document (hours)
        /// </summary>
        public int? RefreshDiscoveryDocDuration { get; set; }
    }
}
