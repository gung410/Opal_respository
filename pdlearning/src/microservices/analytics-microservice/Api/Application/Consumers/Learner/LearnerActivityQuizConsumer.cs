using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.Learner.Messages;
using Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Analytics.Application.Consumers.Learner
{
    [OpalConsumer("microservice.events.learner.user_activity.play_quiz")]
    public class LearnerActivityQuizConsumer : BaseLearnerActivityConsumer<LearnerActivityQuizPayload>
    {
        private readonly IRepository<Learner_UserActivityQuiz> _learnerUserActivityQuizRepo;

        public LearnerActivityQuizConsumer(
            ILoggerFactory loggerFactory,
            IRepository<Learner_UserActivityQuiz> learnerUserActivityQuizRepo,
            IRepository<SAM_UserHistory> userHistoryRepository) : base(loggerFactory, userHistoryRepository)
        {
            _learnerUserActivityQuizRepo = learnerUserActivityQuizRepo;
        }

        public override async Task InternalHandleAsync(BaseLearnerActivityMessage<LearnerActivityQuizPayload> message)
        {
            var latestHistoryItem = await this.GetLatestSAMUserHistoryItem(message);
            if (latestHistoryItem == null)
            {
                return;
            }

            await _learnerUserActivityQuizRepo.InsertAsync(new Learner_UserActivityQuiz()
            {
                UserSessionId = message.SessionId,
                UserId = message.UserId,
                UserHistoryId = latestHistoryItem.Id,
                DepartmentId = latestHistoryItem.DepartmentId,
                ActionName = message.EventName,
                PlayingSessionId = message.Payload.PlayingSessionId,
                FormId = message.Payload.FormId,
                FormAnswerId = message.Payload.FormAnswerId,
                CreatedDate = message.Time
            });
        }
    }
}
