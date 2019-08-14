namespace AspNetCore.IdentityServer4.Auth.Utils.Cache
{
    public interface ICacheKeyFactory
    {
        string KeyPrefixUserProfile { get; }

        string UserProfile(string username);
    }
}
