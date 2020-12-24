namespace AspNetCore.IdentityServer4.Auth.Models
{
    /// <summary>
    /// Consent options
    /// </summary>
    public class ConsentOptions
    {
        /// <summary>
        /// Is enable offline-access
        /// </summary>
        public static bool EnableOfflineAccess = true;

        /// <summary>
        /// Offline-access display name
        /// </summary>
        public static string OfflineAccessDisplayName = "Offline Access";

        /// <summary>
        /// Offline-access description
        /// </summary>
        public static string OfflineAccessDescription = "Access to your applications and resources, even when you are offline";

        /// <summary>
        /// Error msg for must-choose-one-permission
        /// </summary>
        public static readonly string MustChooseOneErrorMessage = "You must pick at least one permission";

        /// <summary>
        /// Error msg for invalid selection
        /// </summary>
        public static readonly string InvalidSelectionErrorMessage = "Invalid selection";
    }
}
