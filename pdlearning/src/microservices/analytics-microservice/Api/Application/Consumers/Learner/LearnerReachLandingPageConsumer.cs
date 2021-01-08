using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.Learner.Messages;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Analytics.Application.Consumers.Learner
{
    [OpalConsumer("microservice.events.learner.user_activity.reach_landing_page")]
    public class LearnerReachLandingPageConsumer : BaseLearnerActivityConsumer<object>
    {
        private readonly ILogger<LearnerReachLandingPageConsumer> _logger;
        private readonly IRepository<Learner_UserActivityLandingPage> _learnerUserActivityLandingPageRepo;

        public LearnerReachLandingPageConsumer(
            ILoggerFactory loggerFactory,
            IRepository<Learner_UserActivityLandingPage> learnerUserActivityLandingPageRepo,
            IRepository<SAM_UserHistory> userHistoryRepository) : base(loggerFactory, userHistoryRepository)
        {
            _logger = loggerFactory.CreateLogger<LearnerReachLandingPageConsumer>();
            _learnerUserActivityLandingPageRepo = learnerUserActivityLandingPageRepo;
        }

        public override async Task InternalHandleAsync(BaseLearnerActivityMessage<object> message)
        {
            var latestHistoryItem = await this.GetLatestSAMUserHistoryItem(message);
            if (latestHistoryItem == null)
            {
                return;
            }

            await _learnerUserActivityLandingPageRepo.InsertAsync(new Learner_UserActivityLandingPage()
            {
                UserSessionId = message.SessionId,
                UserId = message.UserId,
                UserHistoryId = latestHistoryItem.Id,
                DepartmentId = latestHistoryItem.DepartmentId,
                ActionName = message.EventName,
                CreatedDate = message.Time
            });
        }
    }
}
