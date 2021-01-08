using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Infrastructure;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("learningrecord.reflection.created")]
    public class ReflectionCreatedConsumer : OpalMessageConsumer<ReflectionChangedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public ReflectionCreatedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override Task InternalHandleAsync(ReflectionChangedMessage message)
        {
            if (message?.EPortfolio != null && message.Reflection != null && (message.EPortfolio.Visibility == EPortfolioVisibility.Public || message.EPortfolio.IsExperience()))
            {
                var activityType = message.EPortfolio.GetActivityType();
                var reflection = new UserActivity(message.UserId, message.Reflection.Created, message.Id.ToString(), activityType)
                    .WithPortfolioInfo(
                        new Domain.ValueObjects.EPortfolioInfo(
                            message.EPortfolio.Id,
                            message.EPortfolio.UserId,
                            message.EPortfolio.Visibility));

                return _dbContext
                    .ActivityCollection
                    .InsertOneAsync(reflection);
            }

            return Task.CompletedTask;
        }
    }
}
