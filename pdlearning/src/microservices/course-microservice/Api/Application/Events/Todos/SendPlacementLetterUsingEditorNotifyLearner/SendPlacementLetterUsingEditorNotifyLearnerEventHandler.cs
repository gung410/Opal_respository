using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class SendPlacementLetterUsingEditorNotifyLearnerEventHandler : BaseTodoEventHandler<SendPlacementLetterUsingEditorNotifyLearnerEvent, SendPlacementLetterUsingEditorNotifyLearnerPayload>
    {
        public SendPlacementLetterUsingEditorNotifyLearnerEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override string GetMessagePrimary(SendPlacementLetterUsingEditorNotifyLearnerEvent @event)
        {
            return @event.Message;
        }

        protected override string GetPlainTextPrimary(SendPlacementLetterUsingEditorNotifyLearnerEvent @event)
        {
            return @event.PlainText;
        }

        protected override SendPlacementLetterUsingEditorNotifyLearnerPayload GetPayload(SendPlacementLetterUsingEditorNotifyLearnerEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(SendPlacementLetterUsingEditorNotifyLearnerEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
