using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Learner.Application.Commands;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.learner.user_activity.learning_tracking")]
    public class LearningTrackingActivityConsumer : InboxSupportConsumer<LearningTrackingMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;

        public LearningTrackingActivityConsumer(IThunderCqrs thunderCqrs)
        {
            _thunderCqrs = thunderCqrs;
        }

        public Task InternalHandleAsync(LearningTrackingMessage message)
        {
            return _thunderCqrs.SendCommand(new UpdateLearningTrackingCommand
            {
                ItemId = message.Payload.ItemId,
                TrackingAction = message.Payload.TrackingAction,
                TrackingType = message.Payload.TrackingType
            });
        }
    }
}
