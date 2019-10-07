using System.Collections.Generic;
using System.Security.Claims;
using AspNetCore.IdentityServer4.Core.Models;
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
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                new IdentityResources.Address()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new ApiResource[]
            {
                new ApiResource("MyBackendApi1", "My Backend API 1"),
                new ApiResource("MyBackendApi2", "My Backend API 2")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    Enabled = true,
                    ClientId = "OidcBackend",
                    ClientName = "OpenId Connect Client",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        //IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Profile,
                    },
                    AlwaysSendClientClaims = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowAccessTokensViaBrowser = true,
                    IncludeJwtId = true,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 3600,
                },

                new Client
                {
                    Enabled = true,
                    ClientId = "MyBackend",
                    ClientName = "MyBackend Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowedScopes = {
                        "MyBackendApi1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Profile,
                    },
                    AlwaysSendClientClaims = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowAccessTokensViaBrowser = true,
                    IncludeJwtId = true,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 3600,

                    RefreshTokenUsage = TokenUsage.OneTimeOnly, // Or ReUse
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    AbsoluteRefreshTokenLifetime = 360000,
                    SlidingRefreshTokenLifetime = 36000,
                    // IdentityTokenLifetime = 30,
                    // AuthorizationCodeLifetime = 30,
                },

                new Client
                {
                    Enabled = true,
                    ClientId = "PolicyBasedBackend",
                    ClientName = "MyBackend Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowedScopes = {
                        "MyBackendApi2",
                        IdentityServerConstants.StandardScopes.OpenId,
                    },
                    AlwaysSendClientClaims = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowAccessTokensViaBrowser = true,
                    IncludeJwtId = true,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 3600,

                    RefreshTokenUsage = TokenUsage.OneTimeOnly, // Or ReUse
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    AbsoluteRefreshTokenLifetime = 360000,
                    SlidingRefreshTokenLifetime = 36000,
                     
                    ClientClaimsPrefix = string.Empty,
                    //Claims = new Claim[]
                    //{
                    //    // Assign const roles
                    //    new Claim(ClaimTypes.Role, "admin"), // or new Claim(JwtClaimTypes.Role, "admin")
                    //    new Claim(ClaimTypes.Role, "it"),
                    //    // Assign const department
                    //    new Claim(CustomClaimTypes.Department, "Sales")
                    //}
                }

            };
        }
    }
}
