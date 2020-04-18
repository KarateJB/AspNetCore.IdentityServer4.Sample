namespace AspNetCore.IdentityServer4.Core.Models.Config.WebApi
{
    /// <summary>
    /// AppSettings
    /// </summary>
    public class AppSettings: IAppSettings
    {
        /// <summary>
        /// Host options
        /// </summary>
        public HostOptions Host { get; set; }

        /// <summary>
        /// Auth options
        /// </summary>
        public AuthOptions AuthOptions { get; set; }
    }
}
