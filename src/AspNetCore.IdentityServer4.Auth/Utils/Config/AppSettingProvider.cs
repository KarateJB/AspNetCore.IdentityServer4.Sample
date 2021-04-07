using AspNetCore.IdentityServer4.Core.Models.Config.Auth;

namespace AspNetCore.IdentityServer4.Auth.Utils.Config
{
    /// <summary>
    /// App settings static Provider
    /// </summary>
    public static class AppSettingProvider
    {
        /// <summary>
        /// Allowed cross domains
        /// </summary>
        public static string[] AllowedCrossDomains { get; set; }

        /// <summary>
        /// Global options
        /// </summary>
        public static GlobalOptions Global { get; set; }
    }
}
