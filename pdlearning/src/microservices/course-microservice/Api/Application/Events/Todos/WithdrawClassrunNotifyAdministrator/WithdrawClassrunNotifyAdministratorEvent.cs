using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class WithdrawClassrunNotifyAdministratorEvent : BaseTodoEvent<WithdrawClassrunNotifyAdministratorPayload>
    {
        public WithdrawClassrunNotifyAdministratorEvent(Guid createBy, WithdrawClassrunNotifyAdministratorPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:classrun-withdraw:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Withdrawal Pending Confirmation";
            Template = "LearnerWithdrawClassrunPending";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
