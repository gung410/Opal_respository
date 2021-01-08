using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.Learner.Messages;
using Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Analytics.Application.Consumers.Learner
{
    [OpalConsumer("microservice.events.learner.user_activity.learning_tracking")]
    public class LearnerActivityLearningTrackingConsumer : BaseLearnerActivityConsumer<LearnerActivityLearningTrackingPayload>
    {
        private readonly ILogger<LearnerActivityAssignmentConsumer> _logger;
        private readonly IRepository<CCPM_DigitalContentTracking> _ccpmDigitalContentTrackingRepo;

        public LearnerActivityLearningTrackingConsumer(
            ILoggerFactory loggerFactory,
            IRepository<CCPM_DigitalContentTracking> ccpmDigitalContentTrackingRepo,
            IRepository<SAM_UserHistory> userHistoryRepository) : base(loggerFactory, userHistoryRepository)
        {
            _logger = loggerFactory.CreateLogger<LearnerActivityAssignmentConsumer>();
            _ccpmDigitalContentTrackingRepo = ccpmDigitalContentTrackingRepo;
        }

        public override async Task InternalHandleAsync(BaseLearnerActivityMessage<LearnerActivityLearningTrackingPayload> message)
        {
            var latestHistoryItem = await this.GetLatestSAMUserHistoryItem(message);
            if (latestHistoryItem == null)
            {
                return;
            }

            await _ccpmDigitalContentTrackingRepo.InsertAsync(new CCPM_DigitalContentTracking()
            {
                UserId = message.UserId,
                UserHistoryId = latestHistoryItem.Id,
                DepartmentId = latestHistoryItem.DepartmentId,
                DigitalContentId = message.Payload.ItemId,
                Action = message.Payload.TrackingAction
            });
        }
    }
}
