using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core.Models.Config.WebApi;
using AspNetCore.IdentityServer4.Core.Utils.Factory;
using AspNetCore.IdentityServer4.Service.Cache;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AspNetCore.IdentityServer4.WebApi.Utils.Extensions
{
    /// <summary>
    /// ServiceCollections extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Cache service
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Self</returns>
        public static IServiceCollection AddCacheServices(this IServiceCollection services)
        {
            services.AddScoped<ICacheService, RedisService>();
            return services;
        }

        /// <summary>
        /// Add other custom services, utils ...etc
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Self</returns>
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddSingleton<AccessTokenValidator>();
            return services;
        }

        /// <summary>
        /// Add JWT authentication scheme and config
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="appSettings">AppSettings</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, AppSettings appSettings)
        {
            IdentityModelEventSource.ShowPII = true; //Add this line
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // "Bearer"
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // "Bearer"
            }).AddJwtBearer(options =>
            {
                //options.Authority = "https://localhost:6001"; // Base-address of your identityserver
                //options.RequireHttpsMetadata = true;

                string authServerBaseUrl = appSettings.Host?.AuthServer ?? "https://localhost:6001";
                bool isRequireHttpsMetadata = (!string.IsNullOrEmpty(authServerBaseUrl) && authServerBaseUrl.StartsWith("https")) ? true : false;
                options.Authority = string.IsNullOrEmpty(authServerBaseUrl) ? "https://localhost:6001" : authServerBaseUrl;
                options.RequireHttpsMetadata = isRequireHttpsMetadata;
                options.MetadataAddress = $"{authServerBaseUrl}/.well-known/openid-configuration"; // Optional
                options.Audience = appSettings?.AuthOptions?.Audience ?? ApiResources.MyBackendApi2; // API Resource name
                options.TokenValidationParameters.ClockSkew = TimeSpan.Zero; // The JWT security token handler allows for 5 min clock skew in default
                options.BackchannelHttpHandler = AuthMetadataUtils.GetHttpHandler();
                //options.MetadataAddress = $"{authServerBaseUrl}/.well-known/openid-configuration";

                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = (e) =>
                    {
                        // Some callback here ...
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        /// <summary>
        /// Add custom authentication
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="appSettings">AppSettings</param>
        /// <returns>Self</returns>
        public static IServiceCollection AddOpenIdAuthentication(this IServiceCollection services, AppSettings appSettings)
        {
            IdentityModelEventSource.ShowPII = true;

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; // "Cookies"
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // "Cookies"
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect("oidc", options =>
            {
                const string CODE_VERIFIER_KEY = "code_verifier";
                const string CODE_CHALLENGE_KEY = "code_challenge";
                const string CODE_CHALLENGE_METHOD_KEY = "code_challenge_method";

                // Get config values from AppSetting file
                string oidcServerBaseUrl = appSettings?.Host.OidcServer;
                bool isRequireHttpsMetadata = !string.IsNullOrEmpty(oidcServerBaseUrl) && oidcServerBaseUrl.StartsWith("https");
                options.Authority = string.IsNullOrEmpty(oidcServerBaseUrl) ? "https://localhost:6001" : oidcServerBaseUrl;
                options.RequireHttpsMetadata = isRequireHttpsMetadata;
                options.MetadataAddress = $"{oidcServerBaseUrl}/.well-known/openid-configuration";
                options.BackchannelHttpHandler = AuthMetadataUtils.GetHttpHandler();

                options.ClientId = "PkceCodeBackend";
                options.ClientSecret = "secret";
                options.ResponseType = "code";
                options.ResponseMode = "form_post";
                options.CallbackPath = "/signin-oidc";

                options.SaveTokens = true;
                options.Scope.Add(appSettings?.AuthOptions?.Audience);
                options.Scope.Add("offline_access"); // Get refresh token

                options.Events.OnRedirectToIdentityProvider = context =>
                {
                    // only modify requests to the authorization endpoint
                    if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                    {
                        // generate code_verifier
                        var codeVerifier = CryptoRandom.CreateUniqueId(32);

                        // store codeVerifier for later use
                        context.Properties.Items.Remove(CODE_VERIFIER_KEY);
                        context.Properties.Items.Add(CODE_VERIFIER_KEY, codeVerifier);

                        // create code_challenge
                        string codeChallenge;
                        using (var sha256 = SHA256.Create())
                        {
                            var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                            codeChallenge = Base64Url.Encode(challengeBytes);
                        }

                        // add code_challenge and code_challenge_method to request
                        context.ProtocolMessage.Parameters.Remove(CODE_CHALLENGE_KEY);
                        context.ProtocolMessage.Parameters.Remove(CODE_CHALLENGE_METHOD_KEY);
                        context.ProtocolMessage.Parameters.Add(CODE_CHALLENGE_KEY, codeChallenge);
                        context.ProtocolMessage.Parameters.Add(CODE_CHALLENGE_METHOD_KEY, "S256");
                    }

                    return Task.CompletedTask;
                };

                options.Events.OnAuthorizationCodeReceived = context =>
                {
                    // only when authorization code is being swapped for tokens
                    if (context.TokenEndpointRequest?.GrantType == OpenIdConnectGrantTypes.AuthorizationCode)
                    {
                        // get stored code_verifier
                        if (context.Properties.Items.TryGetValue(CODE_VERIFIER_KEY, out var codeVerifier))
                        {
                            // add code_verifier to token request
                            context.TokenEndpointRequest.Parameters.Add(CODE_VERIFIER_KEY, codeVerifier);
                        }
                    }

                    return Task.CompletedTask;
                };
            });

            return services;
        }
    }
}
