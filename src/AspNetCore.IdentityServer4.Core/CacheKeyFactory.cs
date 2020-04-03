namespace AspNetCore.IdentityServer4.Core
{
    /// <summary>
    /// Cache key factory
    /// </summary>
    public static class CacheKeyFactory
    {
        private static string KeyPrefixUserProfile { get; } = "UserProfile";

        /// <summary>
        /// Key for UserProfile
        /// </summary>
        /// <param name="subject">Sub</param>
        /// <returns>Key</returns>
        public static string UserProfile(string subject) => $"{KeyPrefixUserProfile}-{subject}";

        /// <summary>
        /// Key for Auth Server's Signing credential
        /// </summary>
        /// <param name="isDeprecated">Enable getting the key for deprecated Signing credential</param>
        /// <returns>Key</returns>
        public static string SigningCredential(bool isDeprecated = false)
        {
            const string prefix = "SigningCredential";
            return isDeprecated ? $"{prefix}Deprecated" : $"{prefix}";
        }
    }
}
