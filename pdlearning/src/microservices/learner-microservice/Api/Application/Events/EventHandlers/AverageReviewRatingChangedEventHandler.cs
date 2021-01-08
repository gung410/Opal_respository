using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Learner.Application.Models;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Learner.Application.Events.EventHandlers
{
    public class AverageReviewRatingChangedEventHandler : OutboxOpalMqEventHandler<AverageReviewRatingChangedEvent, AverageReviewRatingModel>
    {
        public AverageReviewRatingChangedEventHandler(IOptions<RabbitMQOptions> options, IUserContext userContext, IOutboxQueue outboxQueue) : base(options, userContext, outboxQueue)
        {
        }

        protected override AverageReviewRatingModel GetMQMessagePayload(AverageReviewRatingChangedEvent @event)
        {
            return @event.AverageReviewRating;
        }
    }
}
