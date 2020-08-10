using AspNetCore.IdentityServer4.Mvc.OpenApiSpec;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace AspNetCore.IdentityServer4.Auth.Utils.Config
{
    /// <summary>
    /// Swagger configuration
    /// </summary>
    public class MySwaggerConfig : SwaggerConfig
    {
        /// <summary>
        /// Override Application name
        /// </summary>
        protected override string AppName { get; set; } = "AUTH SERVER";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="provider"></param>
        public MySwaggerConfig(IApiVersionDescriptionProvider provider):base(provider)
        {
        }
    }
}
