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
    [OpalConsumer("csl.events.forum.revision.deleted")]
    public class CSLForumRevisionUpdatedConsumer : ScopedOpalMessageConsumer<CSLForumRevisionUpdatedMessage>
    {
        private readonly ILogger<CSLForumRevisionUpdatedConsumer> _logger;
        private readonly IRepository<CSL_ForumThreadRevision, int> _cslForumRevisionRepository;

        public CSLForumRevisionUpdatedConsumer(ILoggerFactory loggerFactory, IRepository<CSL_ForumThreadRevision, int> cslForumRevisionRepository)
        {
            _logger = loggerFactory.CreateLogger<CSLForumRevisionUpdatedConsumer>();
            _cslForumRevisionRepository = cslForumRevisionRepository;
        }

        public async Task InternalHandleAsync(CSLForumRevisionUpdatedMessage message)
        {
            if (message.Id == null || message.Id == 0)
            {
                _logger.LogError($"CSLForumRevisionUpdatedConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            var cslForumRevision = await _cslForumRevisionRepository.FirstOrDefaultAsync(t => t.Id == message.Id);

            if (cslForumRevision == null)
            {
                _logger.LogWarning($"cslForumRevision {message.Id} does not exist");
                return;
            }

            cslForumRevision.IsLatest = message.IsLatest;
            cslForumRevision.Content = message.Content;
            cslForumRevision.UpdatedDate = Clock.Now;
            await _cslForumRevisionRepository.UpdateAsync(cslForumRevision);
        }
    }
}
