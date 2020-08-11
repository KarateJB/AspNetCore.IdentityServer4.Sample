using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace AspNetCore.IdentityServer4.Mvc.OpenApiSpec
{
    /// <summary>
    /// IApplicationBuilder extensions
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Use Swagger
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="provider">IApiVersionDescriptionProvider</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder UseCustomSwagger(
            this IApplicationBuilder app,
            IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });

            return app;
        }
    }
}
