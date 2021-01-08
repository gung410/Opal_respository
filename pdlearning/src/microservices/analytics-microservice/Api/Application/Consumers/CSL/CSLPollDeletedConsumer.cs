using System;
using System.Text.Json;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.CSL.Messages;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Consumers.CSL
{
    [OpalConsumer("csl.events.poll.deleted")]
    public class CSLPollDeletedConsumer : ScopedOpalMessageConsumer<CSLPollDeletedMessage>
    {
        private readonly ILogger<CSLPollDeletedConsumer> _logger;
        private readonly IRepository<CSL_Poll, int> _pollRepository;

        public CSLPollDeletedConsumer(ILoggerFactory loggerFactory, IRepository<CSL_Poll, int> pollRepository)
        {
            _logger = loggerFactory.CreateLogger<CSLPollDeletedConsumer>();
            _pollRepository = pollRepository;
        }

        public async Task InternalHandleAsync(CSLPollDeletedMessage message)
        {
            if (message.Id == null || message.Id == 0)
            {
                _logger.LogError($"CSLPollDeletedConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            var cslPoll = await _pollRepository.FirstOrDefaultAsync(t => t.Id == message.Id);
            if (cslPoll == null)
            {
                _logger.LogWarning($"The cslPoll {message.Id} does not exist");
                return;
            }

            cslPoll.ToDate = Clock.Now;
            await _pollRepository.UpdateAsync(cslPoll);
        }
    }
}
