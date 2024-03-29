﻿using System.Linq;
using AspNetCore.IdentityServer4.Core.Utils.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.WebApi.Areas.Demo.Controllers
{
    /// <summary>
    /// Demo APIs for Roles based authorization
    /// </summary>
    [Route(RouteFactory.ApiController)]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")] // Specify the scheme name if multiple schemes were set
    public class DemoRoleBasedController : ControllerBase
    {
        private ILogger<DemoRoleBasedController> _logger = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        public DemoRoleBasedController(ILogger<DemoRoleBasedController> logger)
        {
            this._logger = logger;
        }

        [HttpGet]
        [Route("Admin/Get")]
        [Authorize(Roles = "admin")]
        public ActionResult<string> AdminGet()
        {
            this.logtUserClaims();
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
        [Route("Sit/Get")]
        [Authorize(Roles = "sit")]
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

        private void logtUserClaims()
        {
            this.HttpContext.User.Claims.ToList().ForEach(c => this._logger.LogInformation($"Type= {c.Type}, Value= {c.Value}"));
        }
    }
}
