using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.Learner.Messages;
using Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Analytics.Application.Consumers.Learner
{
    [OpalConsumer("microservice.events.learner.user_activity.revisit_mlu")]
    public class LearnerRevisitMLUConsumer : BaseLearnerActivityConsumer<LearnerRevisitMLUPayload>
    {
        private readonly ILogger<LearnerRevisitMLUConsumer> _logger;
        private readonly IRepository<Learner_UserActivityRevisitMlu> _learnerRevisitMLURepo;
        private readonly IRepository<SAM_UserHistory> _userHistoryRepository;

        public LearnerRevisitMLUConsumer(
            ILoggerFactory loggerFactory,
            IRepository<Learner_UserActivityRevisitMlu> learnerRevisitMLURepo,
            IRepository<SAM_UserHistory> userHistoryRepository) : base(loggerFactory, userHistoryRepository)
        {
            _logger = loggerFactory.CreateLogger<LearnerRevisitMLUConsumer>();
            _learnerRevisitMLURepo = learnerRevisitMLURepo;
            _userHistoryRepository = userHistoryRepository;
        }

        public override async Task InternalHandleAsync(BaseLearnerActivityMessage<LearnerRevisitMLUPayload> message)
        {
            var latestHistoryItem = await this.GetLatestSAMUserHistoryItem(message);
            if (latestHistoryItem == null)
            {
                return;
            }

            await _learnerRevisitMLURepo.InsertAsync(new Learner_UserActivityRevisitMlu()
            {
                UserSessionId = message.SessionId,
                UserId = message.UserId,
                UserHistoryId = latestHistoryItem?.Id ?? Guid.Empty,
                DepartmentId = latestHistoryItem?.DepartmentId ?? string.Empty,
                ActionName = message.EventName,
                CourseId = message.Payload.CourseId,
                VisitMode = message.Payload.VisitMode.ToString(),
                CreatedDate = message.Time
            });
        }
    }
}
