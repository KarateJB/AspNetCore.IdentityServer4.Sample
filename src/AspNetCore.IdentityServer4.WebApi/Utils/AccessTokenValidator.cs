using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core.Models.Config.WebApi;
using AspNetCore.IdentityServer4.WebApi.Services;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCore.IdentityServer4.WebApi.Utils
{
    /// <summary>
    /// Access token validator
    /// </summary>
    public class AccessTokenValidator
    {
        private readonly AppSettings appSettings = null;
        private readonly ILogger logger = null;
        private readonly IIdentityClient idsrvClient = null;

        public AccessTokenValidator(
            IOptions<AppSettings> configuration,
            ILogger<AccessTokenValidator> logger,
            IIdentityClient idsrvClient)
        {
            this.appSettings = configuration.Value;
            this.logger = logger;
            this.idsrvClient = idsrvClient;
        }

        /// <summary>
        /// Validate JWT with Auth Server (e.q. Idsrv4)
        /// </summary>
        /// <param name="accessToken">Access token</param>
        /// <returns>True(valid)/False(Invalid)</returns>
        public async Task<Tuple<bool, ClaimsPrincipal>> ValidateAsync(string accessToken)
        {
            const bool validateOk = true;

            #region Get JWKs
            JsonWebKeySetResponse jwks = await this.idsrvClient.GetJwksAsync();

            if (jwks.IsError)
                return await Task.FromResult(new Tuple<bool, ClaimsPrincipal>(!validateOk, null));

            #endregion

            #region Access token validation

            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = this.appSettings.Host.AuthServer,
                ValidAudiences = new[] { appSettings.AuthOptions.Audience },
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero, // See https://stackoverflow.com/a/46231102/7045253
                IssuerSigningKeys = this.getSecurityKeys(jwks.KeySet)
            };

            var handler = new JwtSecurityTokenHandler();

            try
            {
                var user = handler.ValidateToken(accessToken, validationParameters, out SecurityToken validatedToken);
                var isAuthenticated = user.Identities != null && user.Identities.Any(x => x.IsAuthenticated);
                return await Task.FromResult(new Tuple<bool, ClaimsPrincipal>(isAuthenticated, user));
            }
            catch (Exception ex) when (ex is SecurityTokenInvalidSignatureException || ex is SecurityTokenExpiredException)
            {
                // The JWT is invalid
                return await Task.FromResult(new Tuple<bool, ClaimsPrincipal>(!validateOk, null));
            }
            #endregion
        }

        /// <summary>
        /// Convert JsonWebKeySet to List<SecurityKey>
        /// </summary>
        /// <param name="jsonWebKeySet">IdentityModel.Jwk.JsonWebKeySet</param>
        /// <returns>SecurityKey list</returns>
        private List<SecurityKey> getSecurityKeys(IdentityModel.Jwk.JsonWebKeySet jsonWebKeySet)
        {
            var keys = new List<SecurityKey>();

            foreach (var key in jsonWebKeySet.Keys)
            {
                if (key.Kty == "RSA")
                {
                    if (key.X5c != null && key.X5c.Count() > 0)
                    {
                        string certificateString = key.X5c[0];
                        var certificate = new X509Certificate2(Convert.FromBase64String(certificateString));

                        var x509SecurityKey = new X509SecurityKey(certificate)
                        {
                            KeyId = key.Kid
                        };

                        keys.Add(x509SecurityKey);
                    }
                    else if (!string.IsNullOrWhiteSpace(key.E) && !string.IsNullOrWhiteSpace(key.N))
                    {
                        byte[] exponent = fromBase64Url(key.E);
                        byte[] modulus = fromBase64Url(key.N);

                        var rsaParameters = new RSAParameters
                        {
                            Exponent = exponent,
                            Modulus = modulus
                        };

                        var rsaSecurityKey = new RsaSecurityKey(rsaParameters)
                        {
                            KeyId = key.Kid
                        };

                        keys.Add(rsaSecurityKey);
                    }
                    else
                    {
                        throw new Exception("JWK is not available for validating JWT!");
                    }
                }
                else
                {
                    throw new NotImplementedException("Only support RSA for validating JWT!");
                }
            }

            return keys;
        }

        private static byte[] fromBase64Url(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0
                ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded.Replace("_", "/")
                                  .Replace("-", "+");
            var s = Convert.FromBase64String(base64);
            return s;
        }
    }
}
