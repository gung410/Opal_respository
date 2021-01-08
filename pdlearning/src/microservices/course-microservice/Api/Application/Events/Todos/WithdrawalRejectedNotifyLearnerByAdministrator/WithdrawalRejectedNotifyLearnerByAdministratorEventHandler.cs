using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class WithdrawalRejectedNotifyLearnerByAdministratorEventHandler : BaseTodoEventHandler<WithdrawalRejectedNotifyLearnerByAdministratorEvent, WithdrawalRejectedNotifyLearnerByAdministratorPayload>
    {
        public WithdrawalRejectedNotifyLearnerByAdministratorEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override WithdrawalRejectedNotifyLearnerByAdministratorPayload GetPayload(WithdrawalRejectedNotifyLearnerByAdministratorEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(WithdrawalRejectedNotifyLearnerByAdministratorEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
