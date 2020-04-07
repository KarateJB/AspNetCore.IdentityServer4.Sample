namespace AspNetCore.IdentityServer4.Core.Models.Config
{
    /// <summary>
    /// Interface for AppSettings
    /// </summary>
    public interface IAppSettings
    {
        /// <summary>
        /// Host options
        /// </summary>
        HostOptions Host { get; set; }
    }
}
