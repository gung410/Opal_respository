using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cxOrganization.WebServiceAPI.Processor;
using cxOrganization.WebServiceAPI.Processor.Command;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [ApiController]
    [Route("queue")]
    public class QueueController : ControllerBase
    {
        private readonly ILogger<QueueController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ConnectionFactory _factory;
        public QueueController(ILogger<QueueController> logger,
        IConfiguration configuration,
        ConnectionFactory factory,
        IConnection connection, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _configuration = configuration;
            _factory = factory;
            _connection = connection;
            _hostingEnvironment = hostingEnvironment;
        }
        [Route("send_command")]
        [HttpPost]
        public IActionResult SendNotificationCommandAsync(IdpCommand command)
        {
            var json = JsonConvert.SerializeObject(command, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            _factory.RequestedHeartbeat = TimeSpan.FromSeconds(60);

            var publishChannel = _connection.CreateModel();
            var body = Encoding.UTF8.GetBytes(json);
            publishChannel.BasicPublish(exchange: "development-datahub-opal-topic-command",
                                     routingKey: "cxid.authentication.register.user",
                                     basicProperties: null,
                                     body: body);
            publishChannel.Close();

            return Accepted();
        }
    }
}