using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.CSL.Messages;
using Microservice.Analytics.Application.Services.Abstractions;
using Microservice.Analytics.Domain.ValueObject;
using Microsoft.Extensions.Logging;

namespace Microservice.Analytics.Application.Consumers.CSL
{
    [OpalConsumer("csl.events.file.removed")]
    public class CSLFileRemovedConsumer : ScopedOpalMessageConsumer<CSLFileRemovedMessage>
    {
        private readonly ILogger<CSLFileRemovedConsumer> _logger;
        private readonly Func<AnalyticCSLFileObjectModel, IAnalyticsCSLService> _cslFileServiceFunc;

        public CSLFileRemovedConsumer(ILoggerFactory loggerFactory, Func<AnalyticCSLFileObjectModel, IAnalyticsCSLService> cslFileServiceFunc)
        {
            _logger = loggerFactory.CreateLogger<CSLFileRemovedConsumer>();
            _cslFileServiceFunc = cslFileServiceFunc;
        }

        public async Task InternalHandleAsync(CSLFileRemovedMessage message)
        {
            if (message.Id == null || message.Id == 0 || string.IsNullOrEmpty(message.ObjectModel))
            {
                _logger.LogError($"CSLFileRemovedConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            var fileObjectModel = message.ObjectModel.Split("\\").Last();
            var cslFileService = _cslFileServiceFunc((AnalyticCSLFileObjectModel)Enum.Parse(typeof(AnalyticCSLFileObjectModel), fileObjectModel, true));

            if (cslFileService == null)
            {
                _logger.LogError($"CslFileService does not found. {JsonSerializer.Serialize(message)}");
                return;
            }

            await cslFileService.SetToDateAsync(message.Id.Value);
        }
    }
}
