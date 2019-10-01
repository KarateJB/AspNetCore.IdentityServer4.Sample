using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.IdentityServer4.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoRoleBasedController : ControllerBase
    {
        [HttpGet]
        [Route("Admin/Get")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> AdminGet()
        {
            return "Yes, only an Admin can access this API!";
        }

        [HttpGet]
        [Route("User/Get")]
        [Authorize(Roles = "user")]
        public ActionResult<string> UserGet()
        {
            return "Yes, only an User can access this API!";
        }

        [HttpGet]
        [Route("AdminOrUser/Get")]
        [Authorize(Roles = "admin, user")]
        public ActionResult<string> AdminOrUserGet()
        {
            return "Yes, only an Admin or User can access this API!";
        }

        [HttpGet]
        [Route("Everyone/Get")]
        [Authorize]
        public ActionResult<string> AvOneGet()
        {
            return "Everyone can access this API after autenticated!";
        }
    }
}
