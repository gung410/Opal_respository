using System;
using System.Text;
using System.Threading.Tasks;
using Communication.Business.Models.Command;
using Communication.Business.Models.Event;
using cx.datahub.scheduling.jobs.shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Communication.Api.Controllers
{
    [ApiController]
    [Route("hangfire")]
    public class HangfireController : ControllerBase
    {
        private readonly ILogger<HangfireController> _logger;
        private readonly IDigestEmailJob _suspendUserJob;
        public HangfireController(ILogger<HangfireController> logger, IDigestEmailJob suspendUserJob)
        {
            _logger = logger;
            _suspendUserJob = suspendUserJob;
        }

        [Route("enqueue")]
        [HttpPost]
        public async Task<IActionResult> SendNotificationCommandAsync(CommunicationCommand communicationCommand)
        {
            await _suspendUserJob.ExecuteTask(null);
            return Accepted();
        }
        [Route("test")]
        [HttpPost]
        public async Task<IActionResult> TestAsync()
        {
            await _suspendUserJob.ExecuteTask(null);
            return Accepted();
        }


    }
}