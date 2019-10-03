namespace AspNetCore.IdentityServer4.Core
{
    /// <summary>
    /// Cache key factory
    /// </summary>
    public class CacheKeyFactory
    {
        private string KeyPrefixUserProfile { get; } = "UserProfile";
        private string KeyPrefixRoles { get; } = "Roles";

        /// <summary>
        /// Key for UserProfile
        /// </summary>
        /// <param name="subject">Sub</param>
        /// <returns>Key</returns>
        public string UserProfile(string subject) => $"{this.KeyPrefixUserProfile}-{subject}";

        /// <summary>
        /// Key for roles
        /// </summary>
        /// <param name="userName">User's name</param>
        /// <returns>Key</returns>
        public string GetKeyRoles(string userName) => $"{this.KeyPrefixRoles}-{userName}";
    }
}
