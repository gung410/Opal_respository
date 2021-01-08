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
    [OpalConsumer("csl.events.community.deleted")]
    [OpalConsumer("csl.events.subcommunity.deleted")]
    public class CSLCommunityDeletedConsumer : ScopedOpalMessageConsumer<CSLCommunityDeletedMessage>
    {
        private readonly ILogger<CSLCommunityDeletedConsumer> _logger;
        private readonly IRepository<CSL_Space> _cslSpaceRepository;

        public CSLCommunityDeletedConsumer(
            ILoggerFactory loggerFactory,
            IRepository<CSL_Space> cslSpaceRepository)
        {
            _logger = loggerFactory.CreateLogger<CSLCommunityDeletedConsumer>();
            _cslSpaceRepository = cslSpaceRepository;
        }

        public async Task InternalHandleAsync(CSLCommunityDeletedMessage message)
        {
            if (message.Id == Guid.Empty)
            {
                _logger.LogError($"CSLCommunicateDeletedConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            var space = await _cslSpaceRepository.FirstOrDefaultAsync(t => t.Id == message.Id);
            if (space == null)
            {
                _logger.LogWarning($"The space {message.Id} does not exist");
                return;
            }

            space.ToDate = Clock.Now;
            await _cslSpaceRepository.UpdateAsync(space);
        }
    }
}
