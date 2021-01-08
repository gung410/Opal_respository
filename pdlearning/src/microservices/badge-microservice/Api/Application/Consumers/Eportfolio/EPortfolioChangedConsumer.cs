using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Infrastructure;
using MongoDB.Driver;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("learningrecord.eportfolio.updated")]
    public class EPortfolioChangedConsumer : OpalMessageConsumer<EPortfolioChangedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public EPortfolioChangedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override Task InternalHandleAsync(EPortfolioChangedMessage message)
        {
            if (message?.EPortfolio != null)
            {
                var isExperience = message.EPortfolio.IsExperience();

                // Incase the eportfolio type is private showcase.
                if (message.EPortfolio.Visibility == EPortfolioVisibility.Private && isExperience == false)
                {
                    return _dbContext
                        .ActivityCollection
                        .DeleteManyAsync(x => x.EPortfolioInfo.Id == message.EPortfolio.Id);
                }

                // Incase the eportfolio type is experience or public showcase.
                if (message.EPortfolio.Reflections?.Count > 0)
                {
                    var activityType = message.EPortfolio.GetActivityType();
                    var reflections = message
                        .EPortfolio
                        .Reflections
                        .Select(x => new UserActivity(x.UserId, x.Created, message.EPortfolio.Id.ToString(), activityType)
                            .WithPortfolioInfo(new Domain.ValueObjects.EPortfolioInfo(
                                message.EPortfolio.Id,
                                message.EPortfolio.UserId,
                                message.EPortfolio.Visibility)));
                    return _dbContext
                        .ActivityCollection
                        .InsertManyAsync(reflections);
                }
            }

            return Task.CompletedTask;
        }
    }
}
