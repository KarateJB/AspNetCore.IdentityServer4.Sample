using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
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
            string email = string.Empty;
            StringValues authHeaderVal = default(StringValues);

            // Method 1. Use System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler to decode the JWT.
            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out authHeaderVal))
            {
                string bearerTokenPrefix = "Bearer";
                string accessToken = string.Empty;
                string authHeaderStr = authHeaderVal.ToString();
                if (!string.IsNullOrEmpty(authHeaderStr) && authHeaderStr.StartsWith(bearerTokenPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    accessToken = authHeaderStr.Replace(bearerTokenPrefix, string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
                }

                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(accessToken);
                uid = token.Claims.FirstOrDefault(c => c.Type.Equals("sub", StringComparison.OrdinalIgnoreCase))?.Value;
                email = token.Claims.FirstOrDefault(c => c.Type.Equals(JwtClaimTypes.Email))?.Value;
            }

            // 2. Use the System.Security.ClaimsPrinciple from HttpContext.
            var user = context.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                uid = user.Claims.FirstOrDefault(c => c.Type.Equals("sub", StringComparison.OrdinalIgnoreCase))?.Value;
                email = user.Claims.FirstOrDefault(c => c.Type.Equals(JwtClaimTypes.Email))?.Value;
            }

            #region (Optional) Use ActionArguments to get API payload and modify it

            // (Optional) Get payload and modify it
            // MyPayload payload = (MyPayload)context.ActionArguments?.Values.FirstOrDefault(v => v is RequestDto);
            // payload.Uid = uid;
            #endregion

            #region (Optional) Save custom items to HttpContext

            string itemName = "UserProfile";
            context.HttpContext.Items.Add(itemName, new { Id = uid, Email = email });
            #endregion

            await next();
        }
    }
}
