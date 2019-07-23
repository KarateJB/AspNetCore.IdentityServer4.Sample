using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace AspNetCore.IdentityServer4.Auth.Utils.Config
{
    /// <summary>
    /// In memory Identity Server init config
    /// </summary>
    public class InMemoryInitConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new ApiResource[]
            {
                new ApiResource()
                {
                    Name = "MyBackend1",
                    Scopes=
                    {
                        new Scope("MyBackend1"),
                        new Scope(IdentityServerConstants.StandardScopes.Email),
                        //new Scope(IdentityServerConstants.StandardScopes.Profile),
                    }
                },
                new ApiResource("MyBackend2"),
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // client credentials flow client
                new Client
                {
                    Enabled = true,
                    ClientId = "MyBackend",
                    ClientName = "MyBackend Client",
                    AllowedScopes = {
                        "MyBackend1",
                        "MyBackend2",
                        IdentityServerConstants.StandardScopes.Email
                    },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AccessTokenType = AccessTokenType.Jwt,
                    AlwaysSendClientClaims = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowAccessTokensViaBrowser = true,
                    // RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    // RefreshTokenExpiration = TokenExpiration.Sliding,
                    IncludeJwtId = true,
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 3600,
                    // IdentityTokenLifetime = 30,
                    // AuthorizationCodeLifetime = 30,
                    // AbsoluteRefreshTokenLifetime = 30,
                    // SlidingRefreshTokenLifetime = 36000,
                }
            };
        }
    }
}
