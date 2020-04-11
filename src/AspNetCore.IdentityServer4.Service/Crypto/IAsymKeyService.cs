using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core.Models;

namespace AspNetCore.IdentityServer4.Service.Crypto
{
    /// <summary>
    /// Interface for Asymmetric Key Service
    /// </summary>
    public interface IAsymKeyService
    {
        /// <summary>
        /// Create Asymmetric Keys
        /// </summary>
        /// <param name="meta">Key's metadata</param>
        /// <returns>Cipherkey object</returns>
        Task<CipherKey> CreateKeyAsync();

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="publicKey">Public key</param>
        /// <param name="text">Original text</param>
        /// <returns>Cipher text</returns>
        Task<string> EncryptAsync(string publicKey, string text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        Task<string> DecryptAsync(string privateKey, string cipherText);

        /// <summary>
        /// Sign with private key
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="data">Data</param>
        /// <returns>Signed data (Signature)</returns>
        Task<string> SignSignatureAsync(string privateKey, string data);

        /// <summary>
        /// Verify signature with public key
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="data">Original data</param>
        /// <param name="signature">Signed data (Signature)</param>
        /// <returns>True(Verify OK)/False(Verify NG)</returns>
        Task<bool> VerifySignatureAsync(string publicKey, string data, string signature);
    }
}
