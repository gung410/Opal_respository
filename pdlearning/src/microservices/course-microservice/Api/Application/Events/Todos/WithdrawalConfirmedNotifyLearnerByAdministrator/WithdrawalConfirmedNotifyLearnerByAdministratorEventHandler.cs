using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class WithdrawalConfirmedNotifyLearnerByAdministratorEventHandler : BaseTodoEventHandler<WithdrawalConfirmedNotifyLearnerByAdministratorEvent, WithdrawalConfirmedNotifyLearnerByAdministratorPayload>
    {
        public WithdrawalConfirmedNotifyLearnerByAdministratorEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override WithdrawalConfirmedNotifyLearnerByAdministratorPayload GetPayload(WithdrawalConfirmedNotifyLearnerByAdministratorEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(WithdrawalConfirmedNotifyLearnerByAdministratorEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
