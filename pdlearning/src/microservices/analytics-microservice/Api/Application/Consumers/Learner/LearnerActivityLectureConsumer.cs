using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.Learner.Messages;
using Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Analytics.Application.Consumers.Learner
{
    [OpalConsumer("microservice.events.learner.user_activity.start_lecture")]
    [OpalConsumer("microservice.events.learner.user_activity.finish_lecture")]
    public class LearnerActivityLectureConsumer : BaseLearnerActivityConsumer<LearnerActivityLecturePayload>
    {
        private readonly ILogger<LearnerActivityLectureConsumer> _logger;
        private readonly IRepository<Learner_UserActivityLecture> _learnerUserActivityLectureRepo;

        public LearnerActivityLectureConsumer(
            ILoggerFactory loggerFactory,
            IRepository<Learner_UserActivityLecture> learnerUserActivityLectureRepo,
            IRepository<SAM_UserHistory> userHistoryRepository) : base(loggerFactory, userHistoryRepository)
        {
            _logger = loggerFactory.CreateLogger<LearnerActivityLectureConsumer>();
            _learnerUserActivityLectureRepo = learnerUserActivityLectureRepo;
        }

        public override async Task InternalHandleAsync(BaseLearnerActivityMessage<LearnerActivityLecturePayload> message)
        {
            var latestHistoryItem = await this.GetLatestSAMUserHistoryItem(message);
            if (latestHistoryItem == null)
            {
                return;
            }

            await _learnerUserActivityLectureRepo.InsertAsync(new Learner_UserActivityLecture()
            {
                UserSessionId = message.SessionId,
                UserId = message.UserId,
                UserHistoryId = latestHistoryItem.Id,
                DepartmentId = latestHistoryItem.DepartmentId,
                ActionName = message.EventName,
                CourseId = message.Payload.CourseId,
                LectureId = message.Payload.LectureId,
                CreatedDate = message.Time
            });
        }
    }
}
