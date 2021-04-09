using System.Collections.Generic;

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
        /// <remarks>The uri must ends with "/signin-oidc"</remarks>
        public string[] AllowedRedirectUris{ get; set; }

        /// <summary>
        /// Allowed logout redirect uris
        /// </summary>
        /// <remarks>The uri must ends with "/signout-callback-oidc"</remarks>
        public string[] AllowedPostLogoutRedirectUris { get; set; }
    }
}