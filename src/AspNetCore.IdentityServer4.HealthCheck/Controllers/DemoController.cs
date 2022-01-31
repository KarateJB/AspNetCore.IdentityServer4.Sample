using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core.Utils.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AspNetCore.IdentityServer4.HealthCheck.Controllers
{
    /// <summary>
    /// Demo APIs
    /// </summary>
    [Route(RouteFactory.ApiController)]
    [ApiController]
    public class DemoController : ControllerBase
    {
        //private ILogger logger = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        public DemoController(
            ILogger<DemoController> logger)
        {
            //this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return this.Ok();
        }
    }
}
