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
    [OpalConsumer("csl.events.wiki.revision.updated")]
    public class CSLWikiRevisionUpdatedConsumer : ScopedOpalMessageConsumer<CSLWikiRevisionUpdatedMessage>
    {
        private readonly ILogger<CSLWikiRevisionUpdatedConsumer> _logger;
        private readonly IRepository<CSL_WikiPageRevision, int> _cslWikiRevisionRepository;

        public CSLWikiRevisionUpdatedConsumer(ILoggerFactory loggerFactory, IRepository<CSL_WikiPageRevision, int> cslWikiRevisionRepository)
        {
            _logger = loggerFactory.CreateLogger<CSLWikiRevisionUpdatedConsumer>();
            _cslWikiRevisionRepository = cslWikiRevisionRepository;
        }

        public async Task InternalHandleAsync(CSLWikiRevisionUpdatedMessage message)
        {
            if (message.Id == null || message.Id == 0)
            {
                _logger.LogError($"CSLWikiRevisionUpdatedConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            var cslWikiRevision = await _cslWikiRevisionRepository.FirstOrDefaultAsync(t => t.Id == message.Id);

            if (cslWikiRevision == null)
            {
                _logger.LogWarning($"cslWikiRevision {message.Id} does not exist");
                return;
            }

            cslWikiRevision.IsLatest = message.IsLatest;
            cslWikiRevision.Content = message.Content;
            cslWikiRevision.UpdatedDate = Clock.Now;
            await _cslWikiRevisionRepository.UpdateAsync(cslWikiRevision);
        }
    }
}
