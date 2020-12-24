namespace AspNetCore.IdentityServer4.Auth.Models.ViewModels
{
    /// <summary>
    /// External provider
    /// </summary>
    public class ExternalProvider
    {
        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Authentication scheme
        /// </summary>
        public string AuthenticationScheme { get; set; }
    }
}