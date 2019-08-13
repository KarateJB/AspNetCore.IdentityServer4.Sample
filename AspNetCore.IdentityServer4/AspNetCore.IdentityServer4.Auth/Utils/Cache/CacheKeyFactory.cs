namespace AspNetCore.IdentityServer4.Auth.Utils.Cache
{
    public class CacheKeyFactory : ICacheKeyFactory
    {
        public string KeyPrefixUserProfile { get; } = "UserProfile";

        public string UserProfile(string username) => $"{this.KeyPrefixUserProfile}-{username}";
    }
}
