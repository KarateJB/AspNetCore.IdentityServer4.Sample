namespace AspNetCore.IdentityServer4.Core.Models.Config.Auth
{
    /// <summary>
    /// AppSettings
    /// </summary>
    public class AppSettings : IAppSettings
    {
        /// <summary>
        /// Host options
        /// </summary>
        public HostOptions Host { get; set; }

        /// <summary>
        /// Global options
        /// </summary>
        public GlobalOptions Global { get; set; }
    }
}
