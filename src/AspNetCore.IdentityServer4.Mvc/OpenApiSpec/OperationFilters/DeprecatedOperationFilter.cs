using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNetCore.IdentityServer4.Mvc.OpenApiSpec.OperationFilters
{
    /// <summary>
    /// Deprecated API operation filter
    /// </summary>
    public class DeprecatedOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                                    .Union(context.MethodInfo.GetCustomAttributes(true))
                                    .OfType<ApiVersionAttribute>();

            if (attributes != null && attributes.Count() > 0)
            {
                var attr = attributes.ToList().FirstOrDefault(x => x.Deprecated);

                if (attr != null)
                { 
                    // Get group number, e.q. "v2" => "2", "v2.1" => "2.1"
                    double groupNumber = double.Parse(context.ApiDescription?.GroupName.Replace("v", string.Empty, StringComparison.InvariantCultureIgnoreCase));

                    // Current defined API version on ApiVersionAttribute
                    double apiVersion = double.Parse(attr.Versions[0].ToString());

                    if (attr.Deprecated && apiVersion.Equals(groupNumber))
                    {
                        operation.Deprecated = true;

                        // Do other thing...
                    }
                }
            }
        }
    }
}
