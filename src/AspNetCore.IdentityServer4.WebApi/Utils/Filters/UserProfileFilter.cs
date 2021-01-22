using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.WebApi.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace AspNetCore.IdentityServer4.WebApi.Utils.Filters
{
    /// <summary>
    /// UserProfiler filter
    /// </summary>
    public class UserProfileFilter : Attribute, IAsyncActionFilter
    {
        /// <summary>
        /// OnActionExecutionAsync
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string uid = string.Empty;
            StringValues authHeaderVal = default(StringValues);

            // Method 1. Get UID from JWT
            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out authHeaderVal))
            {
                string bearerTokenPrefix = "Bearer";
                string accessToken = string.Empty;
                string authHeaderStr = authHeaderVal.ToString();
                if (!string.IsNullOrEmpty(authHeaderStr) && authHeaderStr.StartsWith(bearerTokenPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    accessToken = authHeaderStr.Replace(bearerTokenPrefix, string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
                }

                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(accessToken);
                uid = token.Claims.FirstOrDefault(c => c.Type.Equals("sub", StringComparison.OrdinalIgnoreCase))?.Value;
            }

            // Method 2. Or Get UID from ActionExecutingContext
            var user = context.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                uid = user.Claims.FirstOrDefault(c => c.Type.Equals("sub", StringComparison.OrdinalIgnoreCase))?.Value;
            }

            // (Optional) Get payload and modify it
            // MyPayload payload = (MyPayload)context.ActionArguments?.Values.FirstOrDefault(v => v is RequestDto);
            // payload.Uid = uid;

            await next();
        }
    }
}
