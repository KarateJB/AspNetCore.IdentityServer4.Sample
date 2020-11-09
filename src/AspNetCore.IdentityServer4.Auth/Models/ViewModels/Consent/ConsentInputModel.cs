using System.Collections.Generic;

namespace AspNetCore.IdentityServer4.Auth.Models.ViewModels
{
    /// <summary>
    /// Consent input model
    /// </summary>
    public class ConsentInputModel
    {
        /// <summary>
        /// Button type
        /// </summary>
        public string Button { get; set; }

        /// <summary>
        /// The scopes that a user agrees to authorize the application to access
        /// </summary>
        public IEnumerable<string> ScopesConsented { get; set; }

        /// <summary>
        /// Is remember the content that the user agreed
        /// </summary>
        public bool RememberConsent { get; set; }

        /// <summary>
        /// Return URL
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
    }
}
