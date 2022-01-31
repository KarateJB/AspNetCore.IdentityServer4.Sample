namespace AspNetCore.IdentityServer4.Core.Models.Config
{
    /// <summary>
    /// Host and Port options
    /// </summary>
    public class HostPortOptions
    {
        /// <summary>
        /// Host name or IP
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; }
    }
}