using System.Net.Http;
using System.Security.Authentication;

namespace AspNetCore.IdentityServer4.WebApi.Utils
{
    /// <summary>
    /// AuthMetadata Utility
    /// </summary>
    public class AuthMetadataUtils
    {
        /// <summary>
        /// Get custom Http client handler
        /// </summary>
        /// <returns>HttpClientHandler</returns>
        public static HttpClientHandler GetHttpHandler()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual; // Optional
            handler.SslProtocols = SslProtocols.Tls12; // Optional
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            // Or write like this
            // handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            return handler;
        }
    }
}
