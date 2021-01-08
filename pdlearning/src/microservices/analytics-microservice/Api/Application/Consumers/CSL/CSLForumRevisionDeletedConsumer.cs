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
    public class CSLForumRevisionDeletedConsumer : ScopedOpalMessageConsumer<CSLForumRevisionDeletedMessage>
    {
        private readonly ILogger<CSLForumRevisionDeletedConsumer> _logger;
        private readonly IRepository<CSL_ForumThreadRevision, int> _cslForumThreadRevisionRepository;

        public CSLForumRevisionDeletedConsumer(ILoggerFactory loggerFactory, IRepository<CSL_ForumThreadRevision, int> cslForumThreadRevisionRepository)
        {
            _logger = loggerFactory.CreateLogger<CSLForumRevisionDeletedConsumer>();
            _cslForumThreadRevisionRepository = cslForumThreadRevisionRepository;
        }

        public async Task InternalHandleAsync(CSLForumRevisionDeletedMessage message)
        {
            if (message.Id == null || message.Id == 0)
            {
                _logger.LogError($"CSLCommunicateDeletedConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            var cslForumThreadRevision = await _cslForumThreadRevisionRepository.FirstOrDefaultAsync(t => t.Id == message.Id);
            if (cslForumThreadRevision == null)
            {
                _logger.LogWarning($"The cslForumThreadRevision {message.Id} does not exist");
                return;
            }

            var now = Clock.Now;
            cslForumThreadRevision.ToDate = now;
            cslForumThreadRevision.UpdatedDate = now;

            await _cslForumThreadRevisionRepository.UpdateAsync(cslForumThreadRevision);
        }
    }
}
