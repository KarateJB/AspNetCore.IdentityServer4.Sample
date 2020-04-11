using System;
using System.Security.Cryptography;

namespace AspNetCore.IdentityServer4.Core.Models
{
    /// <summary>
    /// Signing Credential
    /// </summary>
    public class SigningCredential
    {
        /// <summary>
        /// Key ID
        /// </summary>
        public string KeyId { get; set; }

        /// <summary>
        /// RSA parameters
        /// </summary>
        public RSAParameters Parameters { get; set; }

        /// <summary>
        /// Expire on
        /// </summary>
        public DateTimeOffset ExpireOn { get; set; }
    }
}
