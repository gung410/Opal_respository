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
    [OpalConsumer("csl.events.wiki.revision.deleted")]
    public class CSLWikiRevisionDeletedConsumer : ScopedOpalMessageConsumer<CSLWikiRevisionDeletedMessage>
    {
        private readonly ILogger<CSLWikiRevisionDeletedConsumer> _logger;
        private readonly IRepository<CSL_WikiPageRevision, int> _cslWikiPageRevisionRepository;

        public CSLWikiRevisionDeletedConsumer(ILoggerFactory loggerFactory, IRepository<CSL_WikiPageRevision, int> cslWikiPageRevisionRepository)
        {
            _logger = loggerFactory.CreateLogger<CSLWikiRevisionDeletedConsumer>();
            _cslWikiPageRevisionRepository = cslWikiPageRevisionRepository;
        }

        public async Task InternalHandleAsync(CSLWikiRevisionDeletedMessage message)
        {
            if (message.Id == null || message.Id == 0)
            {
                _logger.LogError($"CSLCommunicateDeletedConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            var wikiRevision = await _cslWikiPageRevisionRepository.FirstOrDefaultAsync(t => t.Id == message.Id);
            if (wikiRevision == null)
            {
                _logger.LogWarning($"WikiPageRevision {message.Id} does not exist");
                return;
            }

            var now = Clock.Now;
            wikiRevision.ToDate = now;
            wikiRevision.UpdatedDate = now;
            await _cslWikiPageRevisionRepository.UpdateAsync(wikiRevision);
        }
    }
}
