using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNetCore.IdentityServer4.Mvc.OpenApiSpec.OperationFilters
{
    /// <summary>
    /// Authorized API operation filter
    /// </summary>
    public class AuthorizationOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Apply operation
        /// </summary>
        /// <param name="operation">Operation</param>
        /// <param name="context">Context</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get Authorize attribute
            var attributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                                    .Union(context.MethodInfo.GetCustomAttributes(true))
                                    .OfType<AuthorizeAttribute>();

            if (attributes != null && attributes.Count() > 0)
            {
                var attr = attributes.ToList()[0];

                // Add response types on secure APIs
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                // Add what should be show inside the security section
                IList<string> securityInfos = new List<string>();
                securityInfos.Add($"{nameof(AuthorizeAttribute.Policy)}:{attr.Policy}");
                securityInfos.Add($"{nameof(AuthorizeAttribute.Roles)}:{attr.Roles}");
                securityInfos.Add($"{nameof(AuthorizeAttribute.AuthenticationSchemes)}:{attr.AuthenticationSchemes}");

                // Set security field for protected routes
                // See https://swagger.io/docs/specification/authentication/bearer-authentication/
                operation.Security = new List<OpenApiSecurityRequirement>()
                {
                    new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "Bearer",
                                Name = "Authorization",
                                In = ParameterLocation.Header,
                            },
                            securityInfos
                        }
                    }
                };
            }
            else
            {
                operation.Security.Clear();
            }
        }
    }
}
