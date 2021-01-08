using Datahub.Queue.Manager.Configurations;
using Datahub.Queue.Manager.Data.MongoDb;
using Datahub.Queue.Manager.Probe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Net;

namespace Datahub.Processor.Controllers
{
    [Produces("application/json")]
    [Route("monitor")]
    public class MonitorController : Controller
    {
        private readonly IOptions<MongoDbSettings> _mongoDbSettings;
        private readonly IConfiguration _configuration;
        private readonly IOptions<RabbitMQSettings> _rabbitMQSetting;
        private readonly ConnectionFactory _factory;

        public MonitorController(IOptions<MongoDbSettings> mongoDbSettings,
            IConfiguration configuration,
            IOptions<RabbitMQSettings> rabbitMQSetting,
            ConnectionFactory factory)
        {
            _mongoDbSettings = mongoDbSettings;
            _configuration = configuration;
            _rabbitMQSetting = rabbitMQSetting;
            _factory = factory;
        }

        /// <summary>
        /// Get status of application
        /// </summary>
        /// <returns></returns>
        [HttpGet("status")]
        public IActionResult CheckStatus()
        {
            var databaseProbe = new DatabaseProbe(_mongoDbSettings).ExecuteProbe();
            var rabbitMQProbe = new RabbitMQProbe(_rabbitMQSetting, _factory).ExecuteProbe();
            if (databaseProbe.IsAlive && rabbitMQProbe.IsAlive)
            {
                return StatusCode((int)HttpStatusCode.OK, databaseProbe);
            }
            return StatusCode((int)HttpStatusCode.ServiceUnavailable, $"MongoDB: {databaseProbe.ToString()} {Environment.NewLine} RabbitMQ: {rabbitMQProbe.ToString()} {Environment.NewLine}");
        }

        [HttpGet("version")]
        public IActionResult Version()
        {
            return StatusCode((int)HttpStatusCode.OK, new { BuildVersion = _configuration["BuildVersion"] });
        }
    }
}