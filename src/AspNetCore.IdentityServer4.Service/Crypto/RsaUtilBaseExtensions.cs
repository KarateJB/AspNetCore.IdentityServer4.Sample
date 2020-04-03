using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using XC.RSAUtil;

namespace AspNetCore.IdentityServer4.Service.Crypto
{
    /// <summary>
    /// RSAUtilBase extensions
    /// </summary>
    public static class RsaUtilBaseExtensions
    {
        // Notice this should be changed only if you modify the connChar parameter in RSAUtilBase.EncryptBigData method
        private static char splitCharForBigData = '$';

        /// <summary>
        /// Use different method to encrypt by data size
        /// </summary>
        /// <param name="rsaUtil">RSAUtilBase</param>
        /// <param name="maxDataSize">Max encrypt data size for RSA</param>
        /// <param name="data">Data</param>
        /// <param name="encryptionPadding">RSAEncryptionPadding</param>
        /// <returns>Encrypted data</returns>
        public static string EncryptByDataSize(
            this RSAUtilBase rsaUtil, int maxDataSize, string data, RSAEncryptionPadding encryptionPadding)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }

            var dataBytes = Encoding.UTF8.GetBytes(data);
            if (dataBytes.Length > maxDataSize)
            {
                return rsaUtil.EncryptBigData(data, encryptionPadding);
            }
            else
            {
                return rsaUtil.Encrypt(data, encryptionPadding);
            }
        }

        /// <summary>
        /// Use different method to decrypt by data size
        /// </summary>
        /// <param name="rsaUtil">RSAUtilBase</param>
        /// <param name="maxDataSize">Max encrypt data size for RSA</param>
        /// <param name="cipherData">Encrypted data</param>
        /// <param name="encryptionPadding">RSAEncryptionPadding</param>
        /// <returns>Decrypted data</returns>
        public static string DecryptByDataSize(
            this RSAUtilBase rsaUtil, int maxDataSize, string cipherData, RSAEncryptionPadding encryptionPadding)
        {
            if (string.IsNullOrEmpty(cipherData))
            {
                return string.Empty;
            }

            var isBigData = cipherData.Contains(splitCharForBigData.ToString());
            if (isBigData)
            {
                return rsaUtil.DecryptBigData(cipherData, encryptionPadding);
            }
            else
            {
                return rsaUtil.Decrypt(cipherData, encryptionPadding);
            }
        }
    }
}

