using AspNetCore.IdentityServer4.WebApi.Utils.Pipelines;
using Microsoft.AspNetCore.Builder;

namespace AspNetCore.IdentityServer4.WebApi.Utils.Extensions
{
    /// <summary>
    /// Pipeline extensions
    /// </summary>
    public static class PipelineExtensions
    {
        /// <summary>
        /// Use TokenExpiredMiddleware
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseInvalidTokenResponse(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<InvalidTokenMiddleware>();
        }
    }
}
