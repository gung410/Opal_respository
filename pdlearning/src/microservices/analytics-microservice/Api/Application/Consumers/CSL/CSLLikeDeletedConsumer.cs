using System;
using System.Text.Json;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.CSL.Messages;
using Microservice.Analytics.Application.Services.Abstractions;
using Microservice.Analytics.Domain.ValueObject;
using Microsoft.Extensions.Logging;

namespace Microservice.Analytics.Application.Consumers.CSL
{
    [OpalConsumer("csl.events.like.deleted")]
    public class CSLLikeDeletedConsumer : ScopedOpalMessageConsumer<CSLLikeDeletedMessage>
    {
        private readonly ILogger<CSLLikeDeletedConsumer> _logger;
        private readonly Func<AnalyticCSLLikeSourceType, IAnalyticsCSLService> _analyticsCSLLikeFunc;

        public CSLLikeDeletedConsumer(
            ILoggerFactory loggerFactory,
            Func<AnalyticCSLLikeSourceType, IAnalyticsCSLService> analyticsCSLLikeFunc)
        {
            _logger = loggerFactory.CreateLogger<CSLLikeDeletedConsumer>();
            _analyticsCSLLikeFunc = analyticsCSLLikeFunc;
        }

        public async Task InternalHandleAsync(CSLLikeDeletedMessage message)
        {
            if (message.Id == null || message.Id == 0)
            {
                _logger.LogError($"CSLLikeDeletedConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            var analyticsCSLLikeService = _analyticsCSLLikeFunc(message.SourceType);
            if (analyticsCSLLikeService == null)
            {
                _logger.LogError($"AnalyticsCSLLikeService does not found: {JsonSerializer.Serialize(message)}");
                return;
            }

            await analyticsCSLLikeService.SetToDateAsync(message.Id.Value);
        }
    }
}
