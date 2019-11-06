using System.Net.Http;
using System.Security.Authentication;

namespace AspNetCore.IdentityServer4.WebApi.Utils
{
    public class AuthMetadataUtils
    {
        public static HttpClientHandler GetHttpHandler()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.SslProtocols = SslProtocols.Tls12;
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            return handler;
        }
    }
}
