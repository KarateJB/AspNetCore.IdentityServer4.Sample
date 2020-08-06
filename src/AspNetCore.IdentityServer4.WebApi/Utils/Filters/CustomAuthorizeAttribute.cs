using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.IdentityServer4.WebApi.Utils.Filters
{
    /// <summary>
    /// Custom Authorization filter
    /// </summary>
    public class CustomAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        /// <summary>
        /// OnAuthorizationAsync
        /// </summary>
        /// <param name="context">AuthorizationFilterContext</param>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            const string BEARER_TOKEN_PREFIX = "Bearer";
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
            string accessToken = string.Empty;

            #region Get Bearer token

            if (!string.IsNullOrEmpty(authorizationHeader) &&
                authorizationHeader.StartsWith(BEARER_TOKEN_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                accessToken = authorizationHeader.Replace(BEARER_TOKEN_PREFIX, string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
            }
            #endregion

            #region Validate JWT

            if (string.IsNullOrEmpty(accessToken))
            {
                context.Result = new UnauthorizedObjectResult($"Bearer token is required");
                return;
            }
            else
            { 
                var tokenValidator = context.HttpContext.RequestServices.GetService<AccessTokenValidator>();
                (var isValid, var user) = await tokenValidator.ValidateAsync(accessToken);
    
                if(!isValid)
                    context.Result = new UnauthorizedObjectResult($"Invalid token");

                return;
            }
            #endregion
        }
    }
}
