namespace AspNetCore.IdentityServer4.Core.Models.Config.Auth
{
    /// <summary>
    /// Open ID options
    /// </summary>
    public class OpenIdOptions
    {
        /// <summary>
        /// Allowed redirect uris
        /// </summary>
        /// <remarks>The uri must ends with "/signin-oidc"
        public string[] AllowedRedirectUris{ get; set; }
    }
}