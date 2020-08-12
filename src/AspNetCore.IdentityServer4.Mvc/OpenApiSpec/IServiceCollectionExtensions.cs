using System.Net;
using System.Reflection;
using AspNetCore.IdentityServer4.Mvc.OpenApiSpec.OperationFilters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
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

                // Set the custom operation filter
                c.OperationFilter<AuthorizationOperationFilter>();
                c.OperationFilter<DeprecatedOperationFilter>();

                // Add JWT Authentication
                // See https://swagger.io/docs/specification/authentication/bearer-authentication/
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = AuthenticationScheme.Bearer,
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = AuthenticationScheme.Bearer,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

                // Note: The following configuration will make ALL APIs as protected!
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {securityScheme, new string[] { }}
                //},);

                // Add Basic Authentication
                var basicSecurityScheme = new OpenApiSecurityScheme
                {
                    Name = "Basic Authentication",
                    Type = SecuritySchemeType.Http,
                    Scheme = AuthenticationScheme.Basic,
                    Reference = new OpenApiReference 
                    { 
                        Id = AuthenticationScheme.Basic, 
                        Type = ReferenceType.SecurityScheme }
                };
                c.AddSecurityDefinition(basicSecurityScheme.Reference.Id, basicSecurityScheme);
            });
            #endregion

            return services;
        }
    }
}
