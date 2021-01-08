using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Reflection;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [Produces("application/json")]
    [Route("monitor")]
    public class MonitorController : Controller
    {
        private readonly IConfiguration _configuration;

        public MonitorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Get status of application
        /// </summary>
        /// <returns></returns>
        [HttpGet("status")]
        public IActionResult CheckStatus()
        {
            return StatusCode((int)HttpStatusCode.OK, new HealthStatus { IsAlive = true });
        }

        [HttpGet("version")]
        public IActionResult Version()
        {
            var version = _configuration["BuildVersion"];
            if (string.IsNullOrEmpty(version))
            {
                version = typeof(Program).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            }
            return StatusCode((int)HttpStatusCode.OK, new { BuildVersion = version });
        }
    }

    internal class HealthStatus
    {
        public bool IsAlive { get; set; }
        public string StatusDetails { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}