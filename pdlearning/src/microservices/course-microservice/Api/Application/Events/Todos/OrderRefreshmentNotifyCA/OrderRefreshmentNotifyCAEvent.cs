using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.Course.Application.Events.Todos
{
    public class OrderRefreshmentNotifyCAEvent : BaseTodoEvent<OrderRefreshmentNotifyCAPayload>
    {
        public OrderRefreshmentNotifyCAEvent(Guid createBy, OrderRefreshmentNotifyCAPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:order-refreshment-notify-ca:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Order Refreshment";
            Template = "OrderRefreshmentNotifyCA";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
