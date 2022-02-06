using AspNetCore.Prometheus.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace AspNetCore.Prometheus.Extensions
{
    /// <summary>
    /// IApplicationBuilder extensions
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Use RequestCounterMw
        /// </summary>
        /// <param name="builder">IApplication instance</param>
        /// <returns>IApplication instance</returns>
        public static IApplicationBuilder UseRequestCounter(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestCounterMw>();
        }
    }
}
