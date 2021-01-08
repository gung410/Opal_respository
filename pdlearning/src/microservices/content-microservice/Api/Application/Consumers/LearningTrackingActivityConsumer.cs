using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Content.Application.Commands;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Consumers
{
    [OpalConsumer("microservice.events.learner.user_activity.learning_tracking")]
    public class LearningTrackingActivityConsumer : ScopedOpalMessageConsumer<LearningTrackingMessage>
    {
        public async Task InternalHandleAsync(LearningTrackingMessage message, IThunderCqrs thunderCqrs)
        {
            var command = new UpdateLearningTrackingCommand
            {
                UserId = message.UserId,
                ItemId = message.Payload.ItemId,
                TrackingAction = message.Payload.TrackingAction,
                TrackingType = message.Payload.TrackingType
            };

            await thunderCqrs.SendCommand(command);
        }
    }
}
