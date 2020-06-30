namespace AspNetCore.IdentityServer4.Core.Models.Config.Auth
{
    /// <summary>
    /// RefresnToken options
    /// </summary>
    public class RefreshTokenOptions
    {
        /// <summary>
        /// Default absolute expiry
        /// </summary>
        public int DefaultAbsoluteExpiry { get; set; }

        /// <summary>
        /// Default sliding expiry
        /// </summary>
        public int DefaultSlidingExpiry { get; set; }
    }
}
