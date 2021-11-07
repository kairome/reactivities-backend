using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("/api/health-check")]
    public class HealthCheckController : BaseController
    {
        [AllowAnonymous]
        [Route("anonymous")]
        public IActionResult PingAnon()
        {
            return Ok();
        }

        [Route("registered")]
        public IActionResult PingRegistered()
        {
            return Ok();
        }
    }
}