namespace AspNetCore.IdentityServer4.Core.Models.Config.Auth
{
    /// <summary>
    /// Access Token Options
    /// </summary>
    public class AccessTokenOptions
    {
        /// <summary>
        /// Default absolute expiry
        /// </summary>
        public int DefaultAbsoluteExpiry { get; set; }

        /// <summary>
        /// Default absolute expiry (ClientCredentials)
        /// </summary>
        public int ClientCredentialsDefaultAbsoluteExpiry { get; set; }
    }
}
