namespace AspNetCore.IdentityServer4.Core.Utils.Factory
{
    /// <summary>
    /// HttpClient name factory
    /// </summary>
    public class HttpClientNameFactory
    {
        /// <summary>
        /// AuthHttpClient's name
        /// </summary>
        public static string AuthHttpClient { get; } = "IdsrvClient_HttpClient";
    }
}
