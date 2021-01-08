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
    [OpalConsumer("csl.events.comment.deleted")]
    public class CSLCommentDeletedConsumer : ScopedOpalMessageConsumer<CSLCommentDeletedMessage>
    {
        private readonly ILogger<CSLCommentDeletedConsumer> _logger;
        private readonly Func<AnalyticCSLCommentThreadType, IAnalyticsCSLService> _analyticCSLCommentServiceFunc;

        public CSLCommentDeletedConsumer(
            ILoggerFactory loggerFactory,
            Func<AnalyticCSLCommentThreadType, IAnalyticsCSLService> analyticCSLCommentServiceFunc)
        {
            _logger = loggerFactory.CreateLogger<CSLCommentDeletedConsumer>();
            _analyticCSLCommentServiceFunc = analyticCSLCommentServiceFunc;
        }

        public async Task InternalHandleAsync(CSLCommentDeletedMessage message)
        {
            if (message.Id == null || message.Id == 0)
            {
                _logger.LogError($"CSLCommunicateDeletedConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            var cslCommentService = _analyticCSLCommentServiceFunc(message.ThreadType);
            if (cslCommentService == null)
            {
                _logger.LogError($"CslCommentService does not found. {JsonSerializer.Serialize(message)}");
                return;
            }

            await cslCommentService.SetToDateAsync(message.Id.Value);
        }
    }
}
