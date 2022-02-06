using System;
using AspNetCore.IdentityServer4.WebApi.Utils.Pipelines;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.WebApi.Utils.Extensions
{
    /// <summary>
    /// IApplicationBuilder Extensions
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Configure and use Exception Handler
        /// </summary>
        /// <param name="builder">IApplicationBuilder instance</param>
        /// <param name="loggerFactory">Logger factory</param>
        public static void ConfigureExceptionHandler(this IApplicationBuilder builder, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("Global Exception Handler");
            builder.UseExceptionHandler(configure =>
            {
                configure.Run(async context =>
                {
                    // Get exception
                    var feature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = feature?.Error ?? new Exception("Internal Server Error");

                    // Logging
                    logger.LogError(exception.Message);

                    // Custom response
                    context.Response.ContentType = "application/json";
                    var json = $"{{\"error\":\"{exception?.Message}\"}}";
                    await context.Response.WriteAsync(json);
                    return;
                });
            });
        }

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
