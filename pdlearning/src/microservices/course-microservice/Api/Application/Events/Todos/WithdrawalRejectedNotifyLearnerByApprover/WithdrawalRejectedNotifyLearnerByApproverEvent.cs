using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class WithdrawalRejectedNotifyLearnerByApproverEvent : BaseTodoEvent<WithdrawalRejectedNotifyLearnerByApproverPayload>
    {
        public WithdrawalRejectedNotifyLearnerByApproverEvent(Guid createBy, WithdrawalRejectedNotifyLearnerByApproverPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:classrun-withdrawal-rejected:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Classrun Withdrawal Request Rejected";
            Template = "ClassRunWithdrawalRejectedByCAO";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
