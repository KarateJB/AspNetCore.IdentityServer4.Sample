using System.Collections.Generic;

namespace AspNetCore.IdentityServer4.Auth.Models.ViewModels
{
    /// <summary>
    /// Consent view model
    /// </summary>
    public class ConsentViewModel : ConsentInputModel
    {
        /// <summary>
        /// Client name
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Client's URL
        /// </summary>
        public string ClientUrl { get; set; }

        /// <summary>
        /// Client logo URL
        /// </summary>
        public string ClientLogoUrl { get; set; }

        /// <summary>
        /// Does allow to remembering consent?
        /// </summary>
        public bool AllowRememberConsent { get; set; }

        /// <summary>
        /// Identity scopes
        /// </summary>
        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }

        /// <summary>
        /// Resource scopes
        /// </summary>
        public IEnumerable<ScopeViewModel> ResourceScopes { get; set; }
    }
}
