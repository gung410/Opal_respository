using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.Learner.Messages;
using Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads;
using Microservice.Analytics.Domain.Entities;
using Microservice.Analytics.Domain.ValueObject;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Analytics.Application.Consumers.Learner
{
    [OpalConsumer("microservice.events.learner.user_activity.reach_pdo_page")]
    [OpalConsumer("microservice.events.learner.user_activity.out_pdo_page")]
    public class LearnerActivityPDOPageConsumer : BaseLearnerActivityConsumer<LearnerActivityPDOPagePayload>
    {
        private readonly ILogger<LearnerActivityPDOPageConsumer> _logger;
        private readonly IRepository<Learner_UserActivityPdoPage> _learnerUserActivityPDOPageRepo;

        public LearnerActivityPDOPageConsumer(
            ILoggerFactory loggerFactory,
            IRepository<Learner_UserActivityPdoPage> learnerUserActivityPDOPageRepo,
            IRepository<SAM_UserHistory> userHistoryRepository) : base(loggerFactory, userHistoryRepository)
        {
            _logger = loggerFactory.CreateLogger<LearnerActivityPDOPageConsumer>();
            _learnerUserActivityPDOPageRepo = learnerUserActivityPDOPageRepo;
        }

        public override async Task InternalHandleAsync(BaseLearnerActivityMessage<LearnerActivityPDOPagePayload> message)
        {
            var latestHistoryItem = await this.GetLatestSAMUserHistoryItem(message);
            if (latestHistoryItem == null)
            {
                return;
            }

            var learnerActivityPDOPage = new Learner_UserActivityPdoPage
            {
                CreatedDate = message.Time,
                UserSessionId = message.SessionId,
                ActionName = message.EventName,
                UserId = message.UserId,
                UserHistoryId = latestHistoryItem.Id,
                DepartmentId = latestHistoryItem.DepartmentId
            };

            if (message.Payload.TrackingType == AnalyticLearnerActivityPDOPageTrackingType.Course
                || message.Payload.TrackingType == AnalyticLearnerActivityPDOPageTrackingType.Microlearning)
            {
                learnerActivityPDOPage.CourseId = message.Payload.ItemId;
            }
            else if (message.Payload.TrackingType == AnalyticLearnerActivityPDOPageTrackingType.DigitalContent)
            {
                learnerActivityPDOPage.DigitalContentId = message.Payload.ItemId;
            }

            await _learnerUserActivityPDOPageRepo.InsertAsync(learnerActivityPDOPage);
        }
    }
}
