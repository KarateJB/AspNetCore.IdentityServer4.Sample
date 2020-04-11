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
    }
}
