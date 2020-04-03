using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Auth.Models.Config;
using AspNetCore.IdentityServer4.Core;
using AspNetCore.IdentityServer4.Core.Models;
using AspNetCore.IdentityServer4.Service.Cache;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCore.IdentityServer4.Auth.Utils.Extensions
{
    /// <summary>
    /// IdentityServerBuilder extensions
    /// </summary>
    public static class IdentityServerBuilderExtensions
    {
        /// <summary>
        /// Add Signing credential from Redis
        /// </summary>
        /// <param name="builder">IIdentityServerBuilder</param>
        /// <param name="configuration">IConfiguration</param>
        /// <returns>IIdentityServerBuilder</returns>
        public static IIdentityServerBuilder AddSigningCredentialFromRedis(
            this IIdentityServerBuilder builder, IConfiguration configuration)
        {
            // Variables
            const double DEFAULT_EXPIRY = 201600; // 201600 sec = 7 days
            var utcNow = DateTimeOffset.UtcNow;
            var redisKeyWorkingSk = CacheKeyFactory.SigningCredential();
            var redisKeyDeprecatedSk = CacheKeyFactory.SigningCredential(isDeprecated: true);

            // RSA key object
            Microsoft.IdentityModel.Tokens.RsaSecurityKey key = null; // The Key for Idsrv

            // Signing credetial object from Redis
            SigningCredential credential = null; // The Signing credential stored in Redis
            List<SigningCredential> deprecatedCredentials = null; // The Deprecated Signing credentials stored in Redis

            // Configuration
            var appSettings = new AppSettings();
            configuration.Bind(appSettings);

            using (ICacheService redis = new RedisService(configuration))
            {
                bool isSigningCredentialExists = redis.GetCache(redisKeyWorkingSk, out credential);

                if (isSigningCredentialExists && credential.ExpireOn >= utcNow)
                {
                    // Use the RSA key pair stored in redis
                    key = CryptoHelper.CreateRsaSecurityKey(credential.Parameters, credential.KeyId);
                }
                else if (isSigningCredentialExists && credential.ExpireOn < utcNow)
                {
                    #region Move the expired Signing credential to Redis's Decprecated-Signing-Credential key

                    _ = redis.GetCache(redisKeyDeprecatedSk, out deprecatedCredentials);

                    if (deprecatedCredentials == null)
                    {
                        deprecatedCredentials = new List<SigningCredential>();
                    }

                    deprecatedCredentials.Add(credential);

                    redis.SaveCache(redisKeyDeprecatedSk, deprecatedCredentials);
                    #endregion

                    #region Clear the expired Signing credential from Redis's Signing-Credential key

                    redis.ClearCache(redisKeyWorkingSk);
                    #endregion

                    // Set flag as False
                    isSigningCredentialExists = false;
                }

                #region If no available Signing credial, create a new one

                if (!isSigningCredentialExists)
                {
                    key = CryptoHelper.CreateRsaSecurityKey();

                    RSAParameters parameters = key.Rsa == null ?
                        parameters = key.Parameters :
                        key.Rsa.ExportParameters(includePrivateParameters: true);

                    credential = new SigningCredential
                    {
                        Parameters = parameters,
                        KeyId = key.KeyId,
                        ExpireOn = DateTimeOffset.UtcNow.AddSeconds(
                            appSettings.Global?.SigningCredential?.AbsoluteExpiry != null ?
                            (double)appSettings.Global?.SigningCredential?.AbsoluteExpiry.Value :
                            DEFAULT_EXPIRY)
                    };

                    // Save to Redis
                    redis.SaveCache(redisKeyWorkingSk, credential);
                }
                #endregion

                // Add the key as the Signing credential for Idsrv
                builder.AddSigningCredential(key, IdentityServerConstants.RsaSigningAlgorithm.RS256);

                // Also add the expired key for clients' old tokens 
                if (redis.GetCache(redisKeyDeprecatedSk, out deprecatedCredentials))
                {
                    IList<SecurityKeyInfo> deprecatedKeyInfos = new List<SecurityKeyInfo>();
                    deprecatedCredentials.ForEach(dc =>
                    {
                    var deprecatedKeyInfo = new SecurityKeyInfo
                    {
                        Key = CryptoHelper.CreateRsaSecurityKey(dc.Parameters, dc.KeyId),
                        SigningAlgorithm = SecurityAlgorithms.RsaSha256
                    };
                    deprecatedKeyInfos.Add(deprecatedKeyInfo);
                    

                    builder.AddValidationKey(deprecatedKeyInfos.ToArray());

                    // Or add only one
                    // builder.AddValidationKey(deprecatedKey, IdentityServerConstants.RsaSigningAlgorithm.RS256));
                });
            }
            }

            return builder;
        }
    }
}
