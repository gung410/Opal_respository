using System;
using System.Text;
using Communication.Business.Models;
using Communication.Business.Models.Command;
using Communication.Business.Models.Event;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Communication.Api.Controllers
{
    [ApiController]
    [Route("notification")]
    public class QueueNotificationController : ControllerBase
    {
        private readonly ILogger<QueueNotificationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ConnectionFactory _factory;
        public QueueNotificationController(ILogger<QueueNotificationController> logger,
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

        [Route("send_notification_command")]
        [HttpPost]
        public IActionResult SendNotificationCommandAsync(CommunicationCommand communicationCommand)
        {
            var exchangeName = _configuration["COMMAND_EXCHANGE_NAME"];

            var json = JsonConvert.SerializeObject(communicationCommand, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            _factory.RequestedHeartbeat = TimeSpan.FromSeconds(60);

            var publishChannel = _connection.CreateModel();
            var body = Encoding.UTF8.GetBytes(json); 
            publishChannel.BasicPublish(exchange: exchangeName,
                                     routingKey: communicationCommand.Routing.Action,
                                     basicProperties: null,
                                     body: body);
            publishChannel.Close();

            return Accepted();
        }

        [Route("send_notification_command_sender")]
        [HttpPost]
        public IActionResult SendNotificationCommandSenderAsync(QueueMessageBase communicationCommand)
        {
            var exchangeName = _configuration["COMMAND_EXCHANGE_NAME"];

            var json = JsonConvert.SerializeObject(communicationCommand, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            _factory.RequestedHeartbeat = TimeSpan.FromSeconds(60);

            var publishChannel = _connection.CreateModel();
            var body = Encoding.UTF8.GetBytes(json);
            publishChannel.BasicPublish(exchange: exchangeName,
                                     routingKey: communicationCommand.Routing.Action,
                                     basicProperties: null,
                                     body: body);
            publishChannel.Close();

            return Accepted();
        }

        [Route("send_notification_event")]
        [HttpPost]
        public IActionResult SendNotificationCommandAsync(CommunicationEvent communicationEvent)
        {
            var exchangeName = _configuration["EVENT_EXCHANGE_NAME"];

            var json = JsonConvert.SerializeObject(communicationEvent, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            _factory.RequestedHeartbeat = TimeSpan.FromSeconds(60);

            var publishChannel = _connection.CreateModel();
            var body = Encoding.UTF8.GetBytes(json);
            publishChannel.BasicPublish(exchange: exchangeName,
                                     routingKey: communicationEvent.Routing.Action,
                                     basicProperties: null,
                                     body: body);
            publishChannel.Close();

            return Accepted();
        }
    }
}