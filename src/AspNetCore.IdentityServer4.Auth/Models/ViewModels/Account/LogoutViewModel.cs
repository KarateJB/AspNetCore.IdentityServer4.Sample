namespace AspNetCore.IdentityServer4.Auth.Models.ViewModels
{
    /// <summary>
    /// Logout view model
    /// </summary>
    public class LogoutViewModel : LogoutInputModel
    {
        /// <summary>
        /// Show logout prompt?
        /// </summary>
        public bool ShowLogoutPrompt { get; set; } = true;
    }
}
