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
    ////[Authorize(AuthenticationSchemes = "Cookies")] // Specify the scheme name if multiple schemes were set
    [Authorize]
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
        /// <param name="user">LDAP user</param>
        /// <returns>JSON object</returns>
        [HttpGet("Login")]
        public async Task<IActionResult> Login()
        {
            return await Task.FromResult(this.View());
        }
        #endregion
    }
}
