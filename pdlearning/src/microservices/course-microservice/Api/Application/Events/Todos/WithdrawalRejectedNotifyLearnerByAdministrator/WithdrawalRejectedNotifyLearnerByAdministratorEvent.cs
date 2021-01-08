using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class WithdrawalRejectedNotifyLearnerByAdministratorEvent : BaseTodoEvent<WithdrawalRejectedNotifyLearnerByAdministratorPayload>
    {
        public WithdrawalRejectedNotifyLearnerByAdministratorEvent(Guid createBy, WithdrawalRejectedNotifyLearnerByAdministratorPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:classrun-withdrawal-rejected:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Classrun Withdrawal Request Rejected";
            Template = "ClassRunWithdrawalRejectedByCA";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
