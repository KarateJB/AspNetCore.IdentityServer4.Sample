using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCore.IdentityServer4.Auth.Utils.Filters
{
    /// <summary>
    /// ActionFilter: Security headers
    /// </summary>
    public class SecurityHeadersAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result;
            if (result is ViewResult)
            {
                #region X-Content-Type-Options
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options"))
                {
                    context.HttpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                }
                #endregion

                #region X-Frame-Options
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options"))
                {
                    context.HttpContext.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                }
                #endregion

                #region Content-Security-Policy
                // HACK: I Disable this header to allow CDN resources, such as bootstrap and etc.
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
                var csp = "default-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';";
                /* Also consider adding upgrade-insecure - requests once you have HTTPS in place for production */
                // csp += "upgrade-insecure-requests;";
                /* Also an example if you need client images to be displayed from twitter */
                // csp += "img-src 'self' https://pbs.twimg.com;";

                // For standards compliant browsers
                //if (!context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
                //{
                //    context.HttpContext.Response.Headers.Add("Content-Security-Policy", csp);
                //}
                //// For IE
                //if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
                //{
                //    context.HttpContext.Response.Headers.Add("X-Content-Security-Policy", csp);
                //}
                #endregion

                #region Referrer-Policy

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
                var referrer_policy = "no-referrer";
                if (!context.HttpContext.Response.Headers.ContainsKey("Referrer-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("Referrer-Policy", referrer_policy);
                }
                #endregion
            }
        }
    }
}
