namespace AspNetCore.IdentityServer4.Auth.Models.Config
{
    /// <summary>
    /// Signing Credential options
    /// </summary>
    public class SigningCredentialOptions
    {
        /// <summary>
        /// Absolute expiry (Seconds)
        /// </summary>
        public int? AbsoluteExpiry { get; set; }
    }
}
