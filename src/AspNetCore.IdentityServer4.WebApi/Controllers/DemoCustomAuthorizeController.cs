using AspNetCore.IdentityServer4.WebApi.Utils.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoCustomAuthorizeController : ControllerBase
    {
        private ILogger logger = null;

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
