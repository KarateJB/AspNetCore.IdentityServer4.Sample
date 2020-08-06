using System.Linq;
using AspNetCore.IdentityServer4.WebApi.Utils.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoPolicyBasedController : ControllerBase
    {
        private ILogger logger = null;

        public DemoPolicyBasedController(
            ILogger<DemoRoleBasedController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("Admin/Get")]
        [Authorize(Policy = "AdminPolicy")]
        public ActionResult<string> AdminGet()
        {
            return "Yes, only an Admin can access this API!";
        }

        [HttpGet]
        [Route("User/Get")]
        [Authorize(Policy = "UserPolicy")]
        public ActionResult<string> UserGet()
        {
            return "Yes, only an User can access this API!";
        }

        [HttpGet]
        [Route("AdminOrUser/Get")]
        [Authorize(Policy = "AdminOrUserPolicy")]
        public ActionResult<string> AdminOrUserGet()
        {
            return "Yes, only an Admin or User can access this API!";
        }

        [HttpGet]
        [Route("AdminAndUser/Get")]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UserPolicy")]
        public ActionResult<string> AdminAndUserGet()
        {
            return "Yes, only as Admin and User can access this API!";
        }

        [HttpGet]
        [Route("Sit/Get")]
        [Authorize(Policy = "SitPolicy")]
        public ActionResult<string> SitGet()
        {
            return "Yes, only an SIT can access this API!";
        }

        [HttpGet]
        [Route("Everyone/Get")]
        [Authorize]
        public ActionResult<string> AvOneGet()
        {
            this.logtUserClaims();
            return "Everyone can access this API after autenticated!";
        }

        [HttpGet]
        [Route("Sales/Get")]
        [Authorize(Policy = "SalesDepartmentPolicy")]
        public ActionResult<string> SalesGet()
        {
            return "Yes, only Sales Department can access this API!";
        }

        [HttpGet]
        [Route("Crm/Get")]
        [Authorize(Policy = "CrmDepartmentPolicy")]
        public ActionResult<string> CrmGet()
        {
            return "Yes, only CRM Deparment can access this API!";
        }

        [HttpGet]
        [Route("Sales/Admin/Get")]
        //[Authorize(Policy = "SalesDepartmentPolicy")]
        //[Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "SalesDepartmentAndAdminPolicy")]
        public ActionResult<string> SalesAdminGet()
        {
            return "Yes, only an Admin of Sales Deparment can access this API!";
        }

        [HttpGet]
        [Route("Sales/AdminOrUser/Get")]
        [Authorize(Policy = "SalesDepartmentAndAdminOrUserPolicy")]
        public ActionResult<string> SalesAdminOrUserGet()
        {
            return "Yes, only an Admin or User of Sales Deparment can access this API!";
        }

        [HttpGet]
        [Route("SalesOrAdmin/Get")]
        [Authorize(Policy = "SalesDepartmentOrAdminPolicy")]
        public ActionResult<string> SalesOrAdminGet()
        {
            return "Yes, only that in Sales Deparment or an Admin can access this API!";
        }

        [HttpGet]
        [Route("AdminOrUserWithCusomHandler/Get")]
        [Authorize(Policy = "AdminOrUserPolicy")]
        [Authorize(Policy = "DoaminAndUsernamePolicy")]
        public ActionResult<string> AdminOrUserWithCusomHandlerGet()
        {
            return "Yes, only an Admin or User can access this API!";
        }

        private void logtUserClaims()
        {
            this.HttpContext.User.Claims.ToList().ForEach(c => this.logger.LogInformation($"Type= {c.Type}, Value= {c.Value}"));
        }
    }
}
