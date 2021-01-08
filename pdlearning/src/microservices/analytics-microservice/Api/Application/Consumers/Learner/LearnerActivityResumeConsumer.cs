using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.Learner.Messages;
using Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Consumers.Learner
{
    [OpalConsumer("microservice.events.learner.user_activity.resume")]
    public class LearnerActivityResumeConsumer : BaseLearnerActivityConsumer<LearnerActivityResumePayload>
    {
        private readonly IRepository<SAM_UserLogin> _userLoginRepository;
        private readonly IRepository<Learner_UserActivityResume> _learnerUserActivityResumeRepository;

        public LearnerActivityResumeConsumer(
            ILoggerFactory loggerFactory,
            IRepository<SAM_UserLogin> userLoginRepository,
            IRepository<SAM_UserHistory> userHistoryRepository,
            IRepository<Learner_UserActivityResume> learnerUserActivityResumeRepository) : base(loggerFactory, userHistoryRepository)
        {
            _userLoginRepository = userLoginRepository;
            _learnerUserActivityResumeRepository = learnerUserActivityResumeRepository;
        }

        public override async Task InternalHandleAsync(BaseLearnerActivityMessage<LearnerActivityResumePayload> message)
        {
            var latestHistoryItem = await this.GetLatestSAMUserHistoryItem(message);
            if (latestHistoryItem == null)
            {
                return;
            }

            await _userLoginRepository.InsertAsync(new SAM_UserLogin()
            {
                LoginDate = Clock.Now,
                Type = "app",
                UserId = message.UserId,
                DepartmentId = latestHistoryItem.DepartmentId,
                UserHistoryId = latestHistoryItem.Id,
                ClientId = message.Payload.ClientId,
                SourceIp = message.SourceIp
            });

            await _learnerUserActivityResumeRepository.InsertAsync(new Learner_UserActivityResume()
            {
                UserId = message.UserId,
                UserHistoryId = latestHistoryItem.Id,
                DepartmentId = latestHistoryItem.DepartmentId,
                ClientId = message.Payload.ClientId,
                CreatedDate = Clock.Now
            });
        }
    }
}
