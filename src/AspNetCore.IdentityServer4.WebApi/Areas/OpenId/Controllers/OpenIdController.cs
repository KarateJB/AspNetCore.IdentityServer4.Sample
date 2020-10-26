using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.WebApi.Areas.Auth.Controllers
{
    /// <summary>
    /// Open ID controller
    /// </summary>
    [Route("[controller]")]
    [Authorize] // DONOT specify the AuthenticationSchemes here
    public class OpenIdController : Controller
    {
        private readonly ILogger logger = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        public OpenIdController(ILogger<OpenIdController> logger)
        {
            this.logger = logger;
        }

        #region Login

        /// <summary>
        /// Login by OIDC
        /// </summary>
        [HttpGet("Login")]
        public async Task<IActionResult> Login()
        {
            return await Task.FromResult(this.View());
        }
        #endregion
    }
}
