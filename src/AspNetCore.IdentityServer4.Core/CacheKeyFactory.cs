namespace AspNetCore.IdentityServer4.Core
{
    /// <summary>
    /// Cache key factory
    /// </summary>
    public class CacheKeyFactory
    {
        private string KeyPrefixUserProfile { get; } = "UserProfile";

        /// <summary>
        /// Key for UserProfile
        /// </summary>
        /// <param name="subject">Sub</param>
        /// <returns>Key</returns>
        public string UserProfile(string subject) => $"{this.KeyPrefixUserProfile}-{subject}";
    }
}
