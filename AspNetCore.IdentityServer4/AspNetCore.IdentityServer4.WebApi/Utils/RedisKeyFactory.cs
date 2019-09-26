namespace AspNetCore.IdentityServer4.WebApi.Utils
{
    /// <summary>
    /// Redis key factory
    /// </summary>
    public class RedisKeyFactory
    {
        private string KeyPrefixRoles { get; } = "Roles";

        /// <summary>
        /// Key for roles
        /// </summary>
        /// <param name="userName">User's name</param>
        /// <returns>Key</returns>
        public string GetKeyRoles(string userName) => $"{this.KeyPrefixRoles}-{userName}";
    }
}
