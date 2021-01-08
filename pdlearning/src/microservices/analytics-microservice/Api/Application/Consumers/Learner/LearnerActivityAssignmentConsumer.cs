using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.Learner.Messages;
using Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Analytics.Application.Consumers.Learner
{
    [OpalConsumer("microservice.events.learner.user_activity.play_assignment")]
    public class LearnerActivityAssignmentConsumer : BaseLearnerActivityConsumer<LearnerActivityAssignmentPayload>
    {
        private readonly ILogger<LearnerActivityAssignmentConsumer> _logger;
        private readonly IRepository<Learner_UserActivityAssignment> _learnerUserActivityAssignmentRepo;

        public LearnerActivityAssignmentConsumer(
            ILoggerFactory loggerFactory,
            IRepository<Learner_UserActivityAssignment> learnerUserActivityAssignmentRepo,
            IRepository<SAM_UserHistory> userHistoryRepository) : base(loggerFactory, userHistoryRepository)
        {
            _logger = loggerFactory.CreateLogger<LearnerActivityAssignmentConsumer>();
            _learnerUserActivityAssignmentRepo = learnerUserActivityAssignmentRepo;
        }

        public override async Task InternalHandleAsync(BaseLearnerActivityMessage<LearnerActivityAssignmentPayload> message)
        {
            var latestHistoryItem = await this.GetLatestSAMUserHistoryItem(message);
            if (latestHistoryItem == null)
            {
                return;
            }

            await _learnerUserActivityAssignmentRepo.InsertAsync(new Learner_UserActivityAssignment()
            {
                UserSessionId = message.SessionId,
                UserId = message.UserId,
                UserHistoryId = latestHistoryItem.Id,
                DepartmentId = latestHistoryItem.DepartmentId,
                ActionName = message.EventName,
                PlayingSessionId = message.Payload.PlayingSessionId,
                AssignmentId = message.Payload.AssignmentId,
                ParticipantAssignmentTrackId = message.Payload.ParticipantAssignmentTrackId,
                CreatedDate = message.Time
            });
        }
    }
}
