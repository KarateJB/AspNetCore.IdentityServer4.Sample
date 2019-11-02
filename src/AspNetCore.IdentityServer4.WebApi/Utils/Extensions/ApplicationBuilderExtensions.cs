using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.WebApi.Utils.Extensions
{
    /// <summary>
    /// ApplicationBuilder Extensions
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("Global Exception Handler");
            app.UseExceptionHandler(configure =>
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
    }
}
