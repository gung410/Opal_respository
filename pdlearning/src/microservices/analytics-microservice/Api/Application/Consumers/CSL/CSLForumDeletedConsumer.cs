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
    [OpalConsumer("csl.events.forum.deleted")]
    public class CSLForumDeletedConsumer : ScopedOpalMessageConsumer<CSLForumDeletedMessage>
    {
        private readonly ILogger<CSLForumDeletedConsumer> _logger;
        private readonly IRepository<CSL_ForumThread, int> _cslForumThreadRepository;

        public CSLForumDeletedConsumer(
            ILoggerFactory loggerFactory,
            IRepository<CSL_ForumThread, int> cslForumThreadRepository)
        {
            _logger = loggerFactory.CreateLogger<CSLForumDeletedConsumer>();
            _cslForumThreadRepository = cslForumThreadRepository;
        }

        public async Task InternalHandleAsync(CSLForumDeletedMessage message)
        {
            if (message.Id == null || message.Id == 0)
            {
                _logger.LogError($"CSLForumDeletedConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            var cslForum = await _cslForumThreadRepository.FirstOrDefaultAsync(t => t.Id == message.Id);
            if (cslForum == null)
            {
                _logger.LogWarning($"CSL Forum {message.Id} does not exist");
                return;
            }

            cslForum.ToDate = Clock.Now;
            await _cslForumThreadRepository.UpdateAsync(cslForum);
        }
    }
}
