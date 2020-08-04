using System;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace AspNetCore.IdentityServer4.WebApi.Utils.Pipelines
{
    /// <summary>
    /// Invalid Token middleware
    /// </summary>
    public class InvalidTokenMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next"></param>
        public InvalidTokenMiddleware(RequestDelegate next)
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
            context.Response.Headers.TryGetValue("WWW-Authenticate", out StringValues authHeader);

            if (!string.IsNullOrEmpty(authHeader))
            {
                switch (authHeader.ToString())
                {
                    case string a when a.Contains("the token expired", StringComparison.InvariantCultureIgnoreCase):
                        context.Response.StatusCode = 498; // Overwrite 401(Unauthorized) to 498(Invalid Token)
                        break;
                    case string b when b.Contains("invalid_token", StringComparison.InvariantCultureIgnoreCase):
                        var idsrvClient = context.RequestServices.GetService<IIdentityClient>();
                        await idsrvClient.RefreshDiscoveryDocAsync();
                        break;
                }
            }
            #endregion
        }
    }
}
