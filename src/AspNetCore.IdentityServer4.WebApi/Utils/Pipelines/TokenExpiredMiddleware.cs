using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.IdentityServer4.WebApi.Utils.Pipelines
{
    /// <summary>
    /// Token expired middleware
    /// </summary>
    public class TokenExpiredMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next"></param>
        public TokenExpiredMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="context">HttpContext</param>
        public async Task InvokeAsync(HttpContext context)
        {
            #region Incoming
            //// Skip incoming request
            #endregion

            await this.next(context);

            #region Outgoing
            context.Response.Headers.TryGetValue("WWW-Authenticate", out StringValues authorizationHeader);
            if (authorizationHeader.ToString().Contains("The token is expired"))
            {
                context.Response.StatusCode = 498; // Overwrite 401(Unauthorized) to 498(Invalid Token)
            }
            #endregion
        }
    }
}
