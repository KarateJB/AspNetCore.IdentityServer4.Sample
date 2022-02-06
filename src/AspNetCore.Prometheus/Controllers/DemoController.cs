using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Prometheus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly ILogger<DemoController> logger;

        public DemoController(ILogger<DemoController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return this.Ok();
        }
    }
}
