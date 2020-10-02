using System;
using System.Collections.Generic;
using System.Security.Claims;
using AspNetCore.IdentityServer4.Core.Models;
using AspNetCore.IdentityServer4.Core.Models.Enum;
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
        /// <summary>
        /// Define identity resources
        /// </summary>
        /// <returns>Identity resources</returns>
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

        /// <summary>
        /// Define API resources
        /// </summary>
        /// <returns>API resources</returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new ApiResource[]
            {
                new ApiResource(ApiResources.MyBackendApi1, "My Backend API 1"),
                new ApiResource(ApiResources.MyBackendApi2, "My Backend API 2")
            };
        }

        /// <summary>
        /// Define clients
        /// </summary>
        /// <returns>Clients</returns>
        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                #region ClientId: "MyBackend", GrandType: "ResourceOwnerPassword"
                new Client
                {
                    Enabled = true,
                    ClientId = AuthClientEnum.MyBackend.ToString(),
                    ClientName = "MyBackend Client", // TODO: AuthClientEnum.MyBackend.GetDescription()
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
                    AccessTokenLifetime = AppSettingProvider.Global?.AccessToken?.DefaultAbsoluteExpiry ?? 3600, // (int)TimeSpan.FromDays(1).TotalSeconds

                    RefreshTokenUsage = TokenUsage.OneTimeOnly, // Or ReUse
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    AbsoluteRefreshTokenLifetime = AppSettingProvider.Global?.RefreshToken?.DefaultAbsoluteExpiry ?? 360000,
                    SlidingRefreshTokenLifetime = AppSettingProvider.Global?.RefreshToken?.DefaultSlidingExpiry ?? 36000,
                    // IdentityTokenLifetime = 30,
                    // AuthorizationCodeLifetime = 30,
                },
                #endregion

                #region ClientId: "PolicyBasedBackend", GrandType: "ResourceOwnerPassword"

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
                        IdentityServerConstants.StandardScopes.Email
                    },
                    AlwaysSendClientClaims = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowAccessTokensViaBrowser = true,
                    IncludeJwtId = true,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = AppSettingProvider.Global?.AccessToken?.DefaultAbsoluteExpiry ?? 3600,

                    RefreshTokenUsage = TokenUsage.OneTimeOnly, // Or ReUse
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    AbsoluteRefreshTokenLifetime = AppSettingProvider.Global?.RefreshToken?.DefaultAbsoluteExpiry ?? 360000,
                    SlidingRefreshTokenLifetime = AppSettingProvider.Global?.RefreshToken?.DefaultSlidingExpiry ?? 36000,

                    ClientClaimsPrefix = string.Empty,
                    //Claims = new Claim[]
                    //{
                    //    // Assign const roles
                    //    new Claim(ClaimTypes.Role, "admin"), // or new Claim(JwtClaimTypes.Role, "admin")
                    //    new Claim(ClaimTypes.Role, "it"),
                    //    // Assign const department
                    //    new Claim(CustomClaimTypes.Department, "Sales")
                    //}
                },
	            #endregion

                #region ClientId: "Resources", GrandType: "ClientCredentials"

                // Client credentials
                new Client
                {
                    Enabled = true,
                    ClientId = "Resources",
                    ClientName = "Resource Owners",
                    AllowedScopes =
                    {
                        "MyBackendApi2",
                    },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AccessTokenType = AccessTokenType.Jwt,
                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    IncludeJwtId = true,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AccessTokenLifetime = AppSettingProvider.Global?.AccessToken?.ClientCredentialsDefaultAbsoluteExpiry ?? 36000,
                },
	            #endregion

                #region ClientId: "pkce_client", GrandType: "code"
                new Client
                {
                    ClientId = "pkce_client",
                    ClientName = "MVC PKCE Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets = {new Secret("acf2ec6fb01a4b698ba240c2b10a0243".Sha256())},
                    RedirectUris = {"https://localhost:5001/signin-oidc"},
                    AllowedScopes = {"openid", "profile", "minitis-api"},
                    AllowOfflineAccess = true,
                    RequirePkce = true,
                    AllowPlainTextPkce = false
                }
	            #endregion
            };
        }
    }
}
