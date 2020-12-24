namespace AspNetCore.IdentityServer4.Auth.Models.ViewModels
{
    /// <summary>
    /// Scope view model
    /// </summary>
    public class ScopeViewModel
    {
        /// <summary>
        /// The scope's value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The scope's display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The scope's description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The scope's emphasize(注重)
        /// </summary>
        public bool Emphasize { get; set; }

        /// <summary>
        /// Is required
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Is checked
        /// </summary>
        public bool Checked { get; set; }
    }
}
