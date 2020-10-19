using System.Collections.Generic;
using AspNetCore.IdentityServer4.Core.Models.Enum;
using AspNetCore.IdentityServer4.Core.Utils.Extensions;
using AspNetCore.IdentityServer4.Core.Utils.Factory;
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
                    ClientName = AuthClientEnum.MyBackend.GetDescription(),
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowedScopes = {
                        ApiResources.MyBackendApi1,
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
                    ClientId = AuthClientEnum.PolicyBasedBackend.ToString(),
                    ClientName = AuthClientEnum.PolicyBasedBackend.GetDescription(),
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowedScopes = {
                        ApiResources.MyBackendApi2,
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
                    ClientId = AuthClientEnum.Resources.ToString(),
                    ClientName = AuthClientEnum.Resources.GetDescription(),
                    AllowedScopes =
                    {
                        ApiResources.MyBackendApi2,
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

                #region ClientId: "PkceCodeBackend", GrandType: "code"
                new Client
                {
                    ClientId = AuthClientEnum.PkceCodeBackend.ToString(),
                    ClientName = AuthClientEnum.PkceCodeBackend.GetDescription(),
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets = { new Secret("acf2ec6fb01a4b698ba240c2b10a0243".Sha256()) },
                    RedirectUris = 
                    {
                        "https://localhost:5001/signin-oidc"
                    },
                    RequireConsent = false, // If enable, will redirect to consent page after sign-in
                    AllowedScopes = 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        ApiResources.MyBackendApi2
                    },
                    AllowOfflineAccess = true,
                    RequirePkce = true,
                    AllowPlainTextPkce = false
                }
	            #endregion
            };
        }
    }
}
