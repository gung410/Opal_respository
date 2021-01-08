using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datahub.Queue.Manager.Commands.QueueConfigurations;
using Datahub.Queue.Manager.Data;
using Datahub.Queue.Manager.Data.MongoDb;
using Datahub.Queue.Manager.Domains;
using Datahub.Queue.Manager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Datahub.Queue.Manager.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class QueueManagerController : ControllerBase
    {
        private readonly IMongoRepository<QueueConfiguration> _mongoRepository;
        private readonly ITransactionalMongoRepository<QueueConfiguration> _transactionalMongoRepository;
        private readonly IMappingService _mapper;
        static IConnection _connection;
        private readonly ILogger _logger;

        public QueueManagerController(IMongoRepository<QueueConfiguration> mongoRepository,
            ITransactionalMongoRepository<QueueConfiguration> transactionalMongoRepository,
            IConnection connection,
            ILogger<QueueManagerController> logger,
            IMappingService mapper
        )
        {
            _mongoRepository = mongoRepository;
            _transactionalMongoRepository = transactionalMongoRepository;
            _mapper = mapper;
            _connection = connection;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetRegistrations(int id)
        {
            return Ok(_mongoRepository.List());
        }

        [HttpPost]
        public async Task<IActionResult> CreateRegistration([FromBody] CreateQueueConfigurationCommand command)
        {
            try
            {
                if (_mongoRepository.Exists(x => x.Exchange == command.Exchange && x.Queue == command.Queue && x.RoutingKey == command.RoutingKey))
                {
                    return NoContent();
                }
                if (_mongoRepository.Exists(x => x.Exchange == command.Exchange && x.ExchangeType != command.ExchangeType))
                {
                    return BadRequest($"{command.Exchange} with another type has been already registered!");
                }
                RegisterQueue(command);
                return Ok(await _mongoRepository.InsertAsync(_mapper.ToQueueConfiguration(command)));
            }
            catch (Exception ex)
            {
                var commandLog = JsonConvert.SerializeObject(command);
                _logger.LogError(ex, $"Error occurred during register Queue: Body({commandLog})");
                throw;
            }
            
        }

        [HttpPost("/bulkinsert")]
        public async Task<IActionResult> CreateRegistrations([FromBody] IEnumerable<CreateQueueConfigurationCommand> commands)
        {
            try
            {
                commands = commands.GroupBy(x => new { x.Exchange, x.Queue, x.RoutingKey }).Select(x => x.First()).ToList();
                foreach (var command in commands)
                {
                    if (_mongoRepository.Exists(x => x.Exchange == command.Exchange && x.Queue == command.Queue && x.RoutingKey == command.RoutingKey))
                    {
                        return NoContent();
                    }

                    if (_mongoRepository.Exists(x => x.Exchange == command.Exchange && x.ExchangeType != command.ExchangeType))
                    {
                        return BadRequest($"{command.Exchange} with another type has been already registered!");
                    }

                    RegisterQueue(command);
                }
                return Ok(await _mongoRepository.InsertManyAsync(_mapper.ToQueueConfigurations(commands)));
            }
            catch (Exception ex)
            {
                var commandsLog = JsonConvert.SerializeObject(commands);
                _logger.LogError(ex, $"Error occurred during register Queues: Body({commandsLog})");
                throw;
            }
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditRegistration(string id, [FromBody] EditQueueConfigurationCommand command)
        {
            var queueConfiguration = _mongoRepository.GetById(new ObjectId(id));
            if (queueConfiguration == null)
            {
                return NotFound();
            }

            queueConfiguration = _mapper.ToQueueConfiguration(queueConfiguration, command);

            await _mongoRepository.UpdateAsync(queueConfiguration);
            return Ok();
        }

        [HttpPost("/ReregisterAllQueues")]
        public async Task<IActionResult> ReregisterAllQueues()
        {
            await Task.Yield();
            var queueConfigurations = _mongoRepository.List();
            if (queueConfigurations != null && queueConfigurations.Any())
            {
                foreach (var queueConfiguration in queueConfigurations)
                {
                    RegisterQueue(new CreateQueueConfigurationCommand()
                    {
                        Exchange = queueConfiguration.Exchange,
                        ExchangeType = queueConfiguration.ExchangeType,
                        Queue = queueConfiguration.Queue,
                        RoutingKey = queueConfiguration.RoutingKey,
                    });
                }
            }
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRegistration(string id, string key, bool isDeleteQueue = false, bool ifUnused = false, bool ifEmpty = false)
        {
            //var queueConfiguration = _mongoRepository.GetById(new ObjectId(id));
            ObjectId.TryParse(id, out var objetcID);
            var queueConfigurations = _mongoRepository.List( x => x.Id == objetcID || x.RoutingKey == key);
            if (queueConfigurations == null || !queueConfigurations.Any())
            {
                return NotFound();
            }
            var queueConfiguration = queueConfigurations.First();
            await _mongoRepository.DeleteAsync(queueConfiguration.Id);
            UnbindQueue(queueConfiguration);
            if (isDeleteQueue)
            {
                RemoveQueue(queueConfiguration, ifUnused, ifEmpty);
            }
            return Ok();
        }

        private static void RegisterQueue(CreateQueueConfigurationCommand queueConfiguration)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: queueConfiguration.Exchange, type: queueConfiguration.ExchangeType, durable: true, autoDelete: false, arguments: null);
                //Set the dead letter exchange for a queue
                Dictionary<string, object> args = new Dictionary<string, object>();
                args.Add("x-dead-letter-exchange", ($"{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}-datahub-{Environment.GetEnvironmentVariable("PROJECT_NAME")}-topic-dead-letter"));
                channel.QueueDeclare(queue: queueConfiguration.Queue, durable: true, exclusive: false, autoDelete: false, arguments: args);
                channel.QueueBind(queue: queueConfiguration.Queue, exchange: queueConfiguration.Exchange, routingKey: queueConfiguration.RoutingKey);
                channel.Close();
            }
        }

        private static void RemoveQueue(QueueConfiguration queueConfiguration, bool ifUnused, bool ifEmpty)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDelete(queueConfiguration.Queue, ifUnused, ifEmpty);
                channel.Close();
            }
        }

        private static void UnbindQueue(QueueConfiguration queueConfiguration)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueUnbind(queueConfiguration.Queue, queueConfiguration.Exchange, queueConfiguration.RoutingKey);
                channel.Close();
            }
        }
    }
}
