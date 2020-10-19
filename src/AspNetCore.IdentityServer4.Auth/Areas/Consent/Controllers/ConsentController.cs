using AspNetCore.IdentityServer4.Auth.Utils.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.Auth.Areas.Consent.Controllers
{
    /// <summary>
    /// Consent controller
    /// </summary>
    [SecurityHeaders]
    [Authorize]
    public class ConsentController : Controller
    {
        private readonly ILogger<ConsentController> logger = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public ConsentController(ILogger<ConsentController> logger)
        {
            this.logger = logger;
        }
    }
}
