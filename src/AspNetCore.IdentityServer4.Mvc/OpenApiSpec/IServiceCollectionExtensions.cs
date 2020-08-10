using System.Reflection;
using AspNetCore.IdentityServer4.Mvc.OpenApiSpec.OperationFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNetCore.IdentityServer4.Mvc.OpenApiSpec
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenApiSpec<T>(this IServiceCollection services)
            where T: SwaggerConfig
        {
            #region API Versioning

            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true; // List supported versons on Http header
                opt.DefaultApiVersion = new ApiVersion(1, 0); // Set the default version
                opt.AssumeDefaultVersionWhenUnspecified = true; // Use the api of default version
                opt.ApiVersionSelector = new CurrentImplementationApiVersionSelector(opt); // Use the api of latest release number
            });
            #endregion

            #region API Document (Swagger)

            services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, T>();
            services.AddSwaggerGen(c =>
            {
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.OperationFilter<AuthorizationOperationFilter>();

                // Set the custom operation filter
                c.OperationFilter<DeprecatedOperationFilter>();
            });
            #endregion

            return services;
        }
    }
}
