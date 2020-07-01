namespace AspNetCore.IdentityServer4.Core.Models.Config.Auth
{
    /// <summary>
    /// Signing Credential options
    /// </summary>
    public class SigningCredentialOptions
    {
        /// <summary>
        /// Expiry, default is 1 year
        /// </summary>
        public DefaultExpiryOptions DefaultExpiry { get; set; }
    }
}
