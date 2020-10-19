using System.Threading.Tasks;
using IdentityServer4.Stores;

namespace AspNetCore.IdentityServer4.Auth.Utils.Extensions
{
    /// <summary>
    /// IClientStore extensions
    /// </summary>
    public static class IClientStoreExtensions
    {
        /// <summary>
        /// Is the client configured to use PKCE.
        /// </summary>
        /// <param name="clientStore">ClientStore</param>
        /// <param name="clientId">Client ID</param>
        /// <returns>True/False</returns>
        public static async Task<bool> IsPkceClientAsync(this IClientStore clientStore, string clientId)
        {
            if (!string.IsNullOrWhiteSpace(clientId))
            {
                var client = await clientStore.FindEnabledClientByIdAsync(clientId);
                return client?.RequirePkce == true;
            }

            return false;
        }
    }
}
