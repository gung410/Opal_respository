using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class AbsenceNotifyLearnerEventHandler : BaseTodoEventHandler<AbsenceNotifyLearnerEvent, AbsenceNotifyLearnerPayload>
    {
        public AbsenceNotifyLearnerEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override string GetModule()
        {
            return "LMM";
        }

        protected override List<ReminderByDto> GetReminderByPrimary(AbsenceNotifyLearnerEvent @event)
        {
            return new List<ReminderByDto>()
            {
                 @event.ReminderByConditions
            };
        }

        protected override AbsenceNotifyLearnerPayload GetPayload(AbsenceNotifyLearnerEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(AbsenceNotifyLearnerEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
