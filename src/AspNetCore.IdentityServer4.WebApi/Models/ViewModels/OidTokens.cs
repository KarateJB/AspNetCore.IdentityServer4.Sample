using System;

namespace AspNetCore.IdentityServer4.WebApi.Models.ViewModels
{
    /// <summary>
    /// OpenID tokens
    /// </summary>
    public class OidTokens
    {
        /// <summary>
        /// id_token
        /// </summary>
        public string IdToken { get; set; }

        /// <summary>
        /// access_token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// refrsh_token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// expires_at
        /// </summary>
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
