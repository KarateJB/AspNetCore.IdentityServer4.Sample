using System.Threading.Tasks;
using AspNetCore.IdentityServer4.WebApi.Utils.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        public async Task<IActionResult> Get()
        {
            return this.Ok();
        }

        [HttpGet("UserProfile")]
        [Authorize(AuthenticationSchemes = "Bearer")] // Specify the scheme name if multiple schemes were set
        [TypeFilter(typeof(UserProfileFilter))]
        public async Task<IActionResult> UserProfile()
        {
            if (this.HttpContext.Items.TryGetValue("UserProfile", out object userProfile))
            {
                this.logger.LogInformation(JsonConvert.SerializeObject(userProfile));
            }
            return this.Ok();
        }
    }
}
