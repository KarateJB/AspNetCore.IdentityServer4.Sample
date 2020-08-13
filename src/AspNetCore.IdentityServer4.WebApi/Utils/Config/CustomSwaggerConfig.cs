using AspNetCore.IdentityServer4.Mvc.OpenApiSpec;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace AspNetCore.IdentityServer4.WevApi.Utils.Config
{
    /// <summary>
    /// Swagger configuration
    /// </summary>
    public class CustomSwaggerConfig : SwaggerConfig
    {
        /// <summary>
        /// Override Application name
        /// </summary>
        protected override string AppName { get; set; } = "Backend";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="provider">IApiVersionDescriptionProvider</param>
        public CustomSwaggerConfig(IApiVersionDescriptionProvider provider):base(provider)
        {
        }
    }
}
