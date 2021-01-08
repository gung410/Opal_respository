using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.Learner.Messages;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Analytics.Application.Consumers.Learner
{
    public abstract class BaseLearnerActivityConsumer<TPayload> : ScopedOpalMessageConsumer<BaseLearnerActivityMessage<TPayload>>
        where TPayload : class
    {
        private readonly ILogger<BaseLearnerActivityConsumer<TPayload>> _logger;
        private readonly IRepository<SAM_UserHistory> _userHistoryRepository;

        protected BaseLearnerActivityConsumer(
            ILoggerFactory loggerFactory,
            IRepository<SAM_UserHistory> userHistoryRepository)
        {
            _logger = loggerFactory.CreateLogger<BaseLearnerActivityConsumer<TPayload>>();
            _userHistoryRepository = userHistoryRepository;
        }

        public abstract Task InternalHandleAsync(BaseLearnerActivityMessage<TPayload> message);

        protected async Task<SAM_UserHistory> GetLatestSAMUserHistoryItem(BaseLearnerActivityMessage<TPayload> message)
        {
            var latestHistoryItem = (await _userHistoryRepository.GetAllListAsync(t => t.UserId == message.UserId))
                .OrderByDescending(t => t.No)
                .FirstOrDefault();

            if (latestHistoryItem == null)
            {
                _logger.LogWarning($"Latest user history with UserId {message.UserId} does not exist");
                return null;
            }

            return latestHistoryItem;
        }
    }
}
