using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using AspNetCore.IdentityServer4.Core.Models;
using AspNetCore.IdentityServer4.Core.Models.Config.Auth;
using AspNetCore.IdentityServer4.Core.Utils;
using AspNetCore.IdentityServer4.Core.Utils.Factory;
using AspNetCore.IdentityServer4.Service.Cache;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace AspNetCore.IdentityServer4.Auth.Utils.Extensions
{
    /// <summary>
    /// IdentityServerBuilder extensions
    /// </summary>
    public static class IdentityServerBuilderExtensions
    {
        private static NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

        /// <summary>
        /// Add Signing credential by file
        /// </summary>
        /// <param name="builder">IIdentityServerBuilder</param>
        /// <param name="appSettings">AppSettings</param>
        /// <returns>IIdentityServerBuilder</returns>
        public static IIdentityServerBuilder AddSigningCredentialsByFile(
            this IIdentityServerBuilder builder, AppSettings appSettings)
        {
            // Variables
            const int DEFAULT_EXPIRY_YEAR = 1;
            const string DIR_NAME_KEYS = "Keys";
            const string FILE_NAME_WORKING_SC = "SigningCredential.rsa";
            const string FILE_NAME_DEPRECATED_SC = "DeprecatedSigningCredentials.rsa";
            var rootDir = Path.Combine(AppContext.BaseDirectory, DIR_NAME_KEYS);
            var workingScDir = Path.Combine(rootDir, FILE_NAME_WORKING_SC);
            var deprecatedScDir = Path.Combine(rootDir, FILE_NAME_DEPRECATED_SC);
            var now = DateTimeOffset.Now;

            // RSA key object
            Microsoft.IdentityModel.Tokens.RsaSecurityKey key = null; // The Key for Idsrv

            // Signing credetial object
            SigningCredential credential = null; // The Signing credential stored in file
            List<SigningCredential> deprecatedCredentials = null; // The Deprecated Signing credentials stored in file

            #region Add exist or new Signing Credentials

            var strWorkingSc = FileUtils.ReadFileAsync(workingScDir).Result;
            var strDeprecatedScs = FileUtils.ReadFileAsync(deprecatedScDir).Result;
            if (!string.IsNullOrEmpty(strWorkingSc))
            {
                // Use the RSA key pair stored in file
                credential = JsonConvert.DeserializeObject<SigningCredential>(strWorkingSc);
                key = CryptoHelper.CreateRsaSecurityKey(credential.Parameters, credential.KeyId);

                // Warning if key expires
                if (credential.ExpireOn < now)
                {
                    logger.Warn($"Auth Server's Signing Credential (ID: {credential.KeyId}) is expired at {credential.ExpireOn.ToLocalTime()}!");
                }
            }
            else
            {
                // Create new RSA key pair
                key = CryptoHelper.CreateRsaSecurityKey();

                RSAParameters parameters = key.Rsa == null ?
                    parameters = key.Parameters :
                    key.Rsa.ExportParameters(includePrivateParameters: true);

                var expireOn = appSettings.Global?.SigningCredential?.DefaultExpiry == null ?
                    now.AddYears(DEFAULT_EXPIRY_YEAR) :
                    appSettings.Global.SigningCredential.DefaultExpiry.GetExpireOn();

                credential = new SigningCredential
                {
                    Parameters = parameters,
                    KeyId = key.KeyId,
                    ExpireOn = expireOn
                };

                // Save to fiile
                FileUtils.SaveFileAsync(workingScDir, JsonConvert.SerializeObject(credential)).Wait();
            }

            // Add the key as the Signing credential for Idsrv
            builder.AddSigningCredential(key, IdentityServerConstants.RsaSigningAlgorithm.RS256);
            #endregion

            #region Add Deprecated Signing Credentials for clients' old tokens

            deprecatedCredentials = string.IsNullOrEmpty(strDeprecatedScs) ? new List<SigningCredential>() : JsonConvert.DeserializeObject<List<SigningCredential>>(strDeprecatedScs);

            IList<SecurityKeyInfo> deprecatedKeyInfos = new List<SecurityKeyInfo>();
            deprecatedCredentials.ForEach(dc =>
            {
                var deprecatedKeyInfo = new SecurityKeyInfo
                {
                    Key = CryptoHelper.CreateRsaSecurityKey(dc.Parameters, dc.KeyId),
                    SigningAlgorithm = SecurityAlgorithms.RsaSha256
                };
                deprecatedKeyInfos.Add(deprecatedKeyInfo);

                // builder.AddValidationKey(deprecatedKey, IdentityServerConstants.RsaSigningAlgorithm.RS256));
            });

            builder.AddValidationKey(deprecatedKeyInfos.ToArray());

            #endregion

            return builder;
        }

        /// <summary>
        /// Add Signing credential by Redis
        /// </summary>
        /// <param name="builder">IIdentityServerBuilder</param>
        /// <param name="appSettings">AppSettings</param>
        /// <returns>IIdentityServerBuilder</returns>
        public static IIdentityServerBuilder AddSigningCredentialByRedis(
            this IIdentityServerBuilder builder, AppSettings appSettings)
        {
            // Variables
            const int DEFAULT_EXPIRY_YEAR = 1;
            var utcNow = DateTimeOffset.UtcNow;
            var redisKeyWorkingSk = CacheKeyFactory.SigningCredential();
            var redisKeyDeprecatedSk = CacheKeyFactory.SigningCredential(isDeprecated: true);

            // RSA key object
            Microsoft.IdentityModel.Tokens.RsaSecurityKey key = null; // The Key for Idsrv

            // Signing credetial object from Redis
            SigningCredential credential = null; // The Signing credential stored in Redis
            List<SigningCredential> deprecatedCredentials = null; // The Deprecated Signing credentials stored in Redis


            using (ICacheService redis = new RedisService(appSettings))
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

                    var expireOn = appSettings.Global?.SigningCredential?.DefaultExpiry == null ?
                                    utcNow.AddYears(DEFAULT_EXPIRY_YEAR) :
                                    appSettings.Global.SigningCredential.DefaultExpiry.GetExpireOn();

                    credential = new SigningCredential
                    {
                        Parameters = parameters,
                        KeyId = key.KeyId,
                        ExpireOn = expireOn
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

                        // builder.AddValidationKey(deprecatedKey, IdentityServerConstants.RsaSigningAlgorithm.RS256));
                    });

                    builder.AddValidationKey(deprecatedKeyInfos.ToArray());
                }
            }

            return builder;
        }

        /// <summary>
        /// Add Signing credential by Certificate
        /// </summary>
        /// <param name="builder">IIdentityServerBuilder</param>
        /// <param name="appSettings"></param>
        /// <param name="isFromCertStore"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddSigningCredentialByCert(
            this IIdentityServerBuilder builder, AppSettings appSettings, bool isFromCertStore = false)
        {
            if (isFromCertStore)
            {
                // See https://www.teilin.net/2018/07/05/self-signed-certificate-and-configuring-identityserver-4-with-certificate/
                using (var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine))
                {
                    certStore.Open(OpenFlags.ReadOnly);
                    var certCollection = certStore.Certificates.Find(
                        X509FindType.FindByThumbprint,
                        string.Empty, // Change this with the thumbprint of your certifiacte
                        false);

                    if (certCollection.Count > 0)
                    {
                        X509Certificate2 cert = certCollection[0];
                        builder.AddSigningCredential(cert);
                    }
                }
            }
            else
            { 
                var rootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Certs");
                var cert = new X509Certificate2(Path.Combine(rootPath, "Docker.pfx"), string.Empty);
                builder.AddSigningCredential(cert);
            }

            return builder;
        }
    }
}
