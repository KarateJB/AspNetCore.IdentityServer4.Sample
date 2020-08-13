using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNetCore.IdentityServer4.Mvc.OpenApiSpec
{
    /// <summary>
    /// Swagger configuration
    /// </summary>
    public class SwaggerConfig : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider provider;
        protected virtual string AppName { get; set; } = string.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="provider"></param>
        public SwaggerConfig(IApiVersionDescriptionProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="options">SwaggerGenOptions</param>
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in this.provider.ApiVersionDescriptions)
            {
                var info = new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = $"{this.AppName} {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                };

                if (description.IsDeprecated)
                {
                    info.Description += " This API version has been deprecated.";
                }

                options.SwaggerDoc(description.GroupName, info);
            }
        }
    }
}
