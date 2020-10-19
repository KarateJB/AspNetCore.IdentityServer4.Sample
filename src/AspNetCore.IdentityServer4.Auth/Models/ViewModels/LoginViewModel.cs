using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.IdentityServer4.Auth.Models.ViewModels
{
    /// <summary>
    /// Login ViewModel
    /// </summary>
    public class LoginViewModel : LoginInputModel
    {
        /// <summary>
        /// Is allow remeber login
        /// </summary>
        public bool AllowRememberLogin { get; set; } = true;

        /// <summary>
        /// Enable local login
        /// </summary>
        public bool EnableLocalLogin { get; set; } = true;

        /// <summary>
        /// External providers
        /// </summary>
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();

        /// <summary>
        /// Visible external providers
        /// </summary>
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !String.IsNullOrWhiteSpace(x.DisplayName));

        /// <summary>
        /// Is external login only
        /// </summary>
        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;

        /// <summary>
        /// External login scheme
        /// </summary>
        public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;
    }
}
