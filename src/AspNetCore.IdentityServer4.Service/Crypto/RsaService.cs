using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core.Models;
using AspNetCore.IdentityServer4.Core.Models.Enum;
using XC.RSAUtil;

namespace AspNetCore.IdentityServer4.Service.Crypto
{
    /// <summary>
    /// RSA Service
    /// </summary>
    /// <remarks>Use RSA2 in default</remarks>
    public class RsaService : IAsymKeyService, IDisposable
    {
        private readonly int keySize;
        private readonly int maxDataSize;
        private readonly HashAlgorithmName hashAlgorithm;
        private readonly RSASignaturePadding signaturePadding;
        private readonly RSAEncryptionPadding encryptionPadding;
        private readonly Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// Constructor using default RSA2 options
        /// </summary>
        public RsaService()
        {
            this.keySize = 2048;
            this.maxDataSize = this.keySize / 8; // RSA is only able to encrypt data with max size = key size
            this.hashAlgorithm = HashAlgorithmName.SHA256;
            this.signaturePadding = RSASignaturePadding.Pss;
            this.encryptionPadding = RSAEncryptionPadding.OaepSHA256;
            this.encoding = Encoding.UTF8;
        }

        /// <summary>
        /// Create RSA key
        /// </summary>
        /// <param name="meta">Key's metadata</param>
        /// <returns>Cipherkey object</returns>
        public async Task<CipherKey> CreateKeyAsync()
        {
            var keys = RsaKeyGenerator.Pkcs8Key(2048, false);
            var privateKey = keys[0];
            var publicKey = keys[1];
            var key = new CipherKey()
            {
                Id = Guid.NewGuid().ToString(),
                KeyType = KeyTypeEnum.RSA,
                PublicKey = publicKey,
                PrivateKey = privateKey
            };
            return await Task.FromResult(key);
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="data">Input data</param>
        /// <returns>Encrypted data</returns>
        public async Task<string> EncryptAsync(string publicKey, string data)
        {
            using (var rsaUtil = new RsaPkcs8Util(this.encoding, publicKey))
            {
                var cipher = rsaUtil.EncryptByDataSize(this.maxDataSize, data, this.encryptionPadding);
                return await Task.FromResult(cipher);
            }
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="cipherData">Encrypted data</param>
        /// <returns>Decrypted data</returns>
        public async Task<string> DecryptAsync(string privateKey, string cipherData)
        {
            using (var rsaUtil = new RsaPkcs8Util(this.encoding, string.Empty, privateKey))
            {
                var text = rsaUtil.DecryptByDataSize(this.maxDataSize, cipherData, this.encryptionPadding);
                return await Task.FromResult(text);
            }
        }

        /// <summary>
        /// Sign with private key
        /// </summary>
        /// <param name="privateKey">Private key</param>
        /// <param name="data">Data</param>
        /// <returns></returns>
        public async Task<string> SignSignatureAsync(string privateKey, string data)
        {
            using (var rsaUtil = new RsaPkcs8Util(this.encoding, string.Empty, privateKey, this.keySize))
            {
                var isOk = rsaUtil.SignData(data, HashAlgorithmName.SHA256, this.signaturePadding);
                return await Task.FromResult(isOk);
            }
        }

        /// <summary>
        /// Verify signature with public key
        /// </summary>
        /// <param name="publicKey">Public key</param>
        /// <param name="data">Original data</param>
        /// <param name="signature">Signed data (Signature)</param>
        /// <returns>True(Verify OK)/False(Verify NG)</returns>
        public async Task<bool> VerifySignatureAsync(string publicKey, string data, string signature)
        {
            using (var rsaUtil = new RsaPkcs8Util(this.encoding, publicKey, string.Empty, this.keySize))
            {
                var isOk = rsaUtil.VerifyData(data, signature, this.hashAlgorithm, this.signaturePadding);
                return await Task.FromResult(isOk);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }
    }
}
