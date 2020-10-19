using AspNetCore.IdentityServer4.WebApi.Utils.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.WebApi.Areas.Demo.Controllers
{
    /// <summary>
    /// Demo APIs for custom authorization
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DemoCustomAuthorizeController : ControllerBase
    {
        private ILogger logger = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        public DemoCustomAuthorizeController(
            ILogger<DemoCustomAuthorizeController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [CustomAuthorize]
        public ActionResult Get()
        {
            return this.Ok();
        }
    }
}
