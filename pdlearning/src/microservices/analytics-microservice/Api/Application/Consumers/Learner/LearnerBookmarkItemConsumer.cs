using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.Learner.Messages;
using Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads;
using Microservice.Analytics.Application.Services.Abstractions;
using Microservice.Analytics.Domain.Entities;
using Microservice.Analytics.Domain.ValueObject;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Analytics.Application.Consumers.Learner
{
    [OpalConsumer("microservice.events.learner.user_activity.bookmark_item")]
    public class LearnerBookmarkItemConsumer : BaseLearnerActivityConsumer<LearnerBookmarkItemPayload>
    {
        private readonly ILogger<LearnerBookmarkItemConsumer> _logger;
        private readonly Func<AnalyticLearnerBookmarkItemType, IAnalyticsLearnerService> _analyticLearnerBookmarkServiceFunc;

        public LearnerBookmarkItemConsumer(
            ILoggerFactory loggerFactory,
            IRepository<SAM_UserHistory> userHistoryRepository,
            Func<AnalyticLearnerBookmarkItemType, IAnalyticsLearnerService> analyticLearnerBookmarkServiceFunc) : base(loggerFactory, userHistoryRepository)
        {
            _logger = loggerFactory.CreateLogger<LearnerBookmarkItemConsumer>();
            _analyticLearnerBookmarkServiceFunc = analyticLearnerBookmarkServiceFunc;
        }

        public override async Task InternalHandleAsync(BaseLearnerActivityMessage<LearnerBookmarkItemPayload> message)
        {
            var analyticLearnerBookmarkItemService = _analyticLearnerBookmarkServiceFunc(message.Payload.ItemType);
            if (analyticLearnerBookmarkItemService == null)
            {
                _logger.LogWarning($"Missing learner bookmark service for {message.Payload.ItemType.ToString()}");
                return;
            }

            var latestHistoryItem = await this.GetLatestSAMUserHistoryItem(message);
            if (latestHistoryItem == null)
            {
                return;
            }

            await analyticLearnerBookmarkItemService.CreateOrSetToDateBookmarkItem(
                message.Payload.ItemId,
                message.UserId,
                message.Payload.IsUnBookmark,
                message.Payload.ItemType,
                message.Time,
                latestHistoryItem);
        }
    }
}
