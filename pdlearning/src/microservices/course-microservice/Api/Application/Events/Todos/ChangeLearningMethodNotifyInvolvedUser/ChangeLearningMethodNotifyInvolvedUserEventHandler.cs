using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class ChangeLearningMethodNotifyInvolvedUserEventHandler : BaseTodoEventHandler<ChangeLearningMethodNotifyInvolvedUserEvent, ChangeLearningMethodNotifyInvolvedUserPayload>
    {
        public ChangeLearningMethodNotifyInvolvedUserEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override ChangeLearningMethodNotifyInvolvedUserPayload GetPayload(ChangeLearningMethodNotifyInvolvedUserEvent @event)
        {
            return @event.Payload;
        }

        protected override string GetModule()
        {
            return "LMM";
        }

        protected override List<ReceiverDto> GetPrimary(ChangeLearningMethodNotifyInvolvedUserEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
