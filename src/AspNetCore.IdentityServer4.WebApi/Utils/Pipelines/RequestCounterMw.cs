using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Prometheus;

namespace AspNetCore.IdentityServer4.WebApi.Utils.Pipelines
{
    /// <summary>
    /// Request Counter middleware
    /// </summary>
    public class RequestCounterMw
    {
        private readonly RequestDelegate next;
        public RequestCounterMw(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Http Context
            var counter = Metrics.CreateCounter("RequestCounter", "Count request",
            new CounterConfiguration
            {
                LabelNames = new[] { "method", "endpoint" }
            });
            // method: GET, POST etc.
            // endpoint: Requested path

            counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
            await next(context);
        }
    }
}
