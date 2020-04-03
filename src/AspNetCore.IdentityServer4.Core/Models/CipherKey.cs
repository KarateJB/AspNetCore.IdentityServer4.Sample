using System;
using AspNetCore.IdentityServer4.Core.Models.Enum;

namespace AspNetCore.IdentityServer4.Core.Models
{
    /// <summary>
    /// Key information
    /// </summary>
    [Serializable]
    public class CipherKey
    {
        /// <summary>
        /// GUID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Key Type
        /// </summary>
        public KeyTypeEnum KeyType { get; set; }

        /// <summary>
        /// Key | Public key in base64
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Key | Private key in base64
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>The masked key information</returns>
        public override string ToString()
        {
            var info =
                $"({this.Id}): Public key {this.PublicKey},(Private key): {this.PrivateKey}";
            return info;
        }
    }
}

