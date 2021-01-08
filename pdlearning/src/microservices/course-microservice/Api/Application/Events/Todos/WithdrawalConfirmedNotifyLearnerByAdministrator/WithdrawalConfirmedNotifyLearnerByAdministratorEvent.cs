using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class WithdrawalConfirmedNotifyLearnerByAdministratorEvent : BaseTodoEvent<WithdrawalConfirmedNotifyLearnerByAdministratorPayload>
    {
        public WithdrawalConfirmedNotifyLearnerByAdministratorEvent(Guid createBy, WithdrawalConfirmedNotifyLearnerByAdministratorPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:classrun-withdrawal-confirmed:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Classrun Withdrawal Request Confirmed";
            Template = "ClassRunWithdrawalConfirmedByCA";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
