using System;
using System.ComponentModel;

namespace AspNetCore.IdentityServer4.WebApi.Models.ViewModels
{
    /// <summary>
    /// OpenID User informations, including tokens
    /// </summary>
    public class OidUserInfo
    {
        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// id_token
        /// </summary>
        [DisplayName("id_token")]
        public string IdToken { get; set; }

        /// <summary>
        /// access_token
        /// </summary>
        [DisplayName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// refrsh_token
        /// </summary>

        [DisplayName("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// expires_at
        /// </summary>
        [DisplayName("expires_at")]
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
