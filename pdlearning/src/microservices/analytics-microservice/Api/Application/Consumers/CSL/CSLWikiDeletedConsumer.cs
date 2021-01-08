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
    [OpalConsumer("csl.events.wiki.deleted")]
    public class CSLWikiDeletedConsumer : ScopedOpalMessageConsumer<CSLWikiDeletedMessage>
    {
        private readonly ILogger<CSLWikiDeletedConsumer> _logger;
        private IRepository<CSL_WikiPage, int> _cslWikiRepository;

        public CSLWikiDeletedConsumer(ILoggerFactory loggerFactory, IRepository<CSL_WikiPage, int> cslWikiRepository)
        {
            _logger = loggerFactory.CreateLogger<CSLWikiDeletedConsumer>();
            _cslWikiRepository = cslWikiRepository;
        }

        public async Task InternalHandleAsync(CSLWikiDeletedMessage message)
        {
            if (message.Id == null || message.Id == 0)
            {
                _logger.LogError($"CSLWikiDeletedConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            var cslWiki = await _cslWikiRepository.FirstOrDefaultAsync(t => t.Id == message.Id);
            if (cslWiki == null)
            {
                _logger.LogWarning($"The CSL wiki {message.Id} does not exist");
                return;
            }

            cslWiki.ToDate = Clock.Now;
            await _cslWikiRepository.UpdateAsync(cslWiki);
        }
    }
}
