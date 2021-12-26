using AspNetCore.IdentityServer4.Core.Utils.Factory;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.IdentityServer4.Auth.Areas.Info.Controllers
{
    /// <summary>
    /// Information controller
    /// </summary>
    [Route(RouteFactory.ApiController)]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return this.Ok("Auth Server 0.1.0.0");
        }
    }
}
