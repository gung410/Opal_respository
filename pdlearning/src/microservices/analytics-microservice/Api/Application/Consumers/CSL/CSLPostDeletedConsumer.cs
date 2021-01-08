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
    [OpalConsumer("csl.events.post.deleted")]
    public class CSLPostDeletedConsumer : ScopedOpalMessageConsumer<CSLPostDeletedMessage>
    {
        private readonly ILogger<CSLPostDeletedConsumer> _logger;
        private readonly IRepository<CSL_Post, int> _cslPostRepository;

        public CSLPostDeletedConsumer(
            ILoggerFactory loggerFactory,
            IRepository<CSL_Post, int> cslPostRepository)
        {
            _logger = loggerFactory.CreateLogger<CSLPostDeletedConsumer>();
            _cslPostRepository = cslPostRepository;
        }

        public async Task InternalHandleAsync(CSLPostDeletedMessage message)
        {
            if (message.Id == null || message.Id == 0)
            {
                _logger.LogError($"CSLPostDeletedConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            var post = await _cslPostRepository.FirstOrDefaultAsync(t => t.Id == message.Id);
            if (post == null)
            {
                _logger.LogWarning($"The Post {message.Id} does not exist");
                return;
            }

            post.ToDate = Clock.Now;
            await _cslPostRepository.UpdateAsync(post);
        }
    }
}
