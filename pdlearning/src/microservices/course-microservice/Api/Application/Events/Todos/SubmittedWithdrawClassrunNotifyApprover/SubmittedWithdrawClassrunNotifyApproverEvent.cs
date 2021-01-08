using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class SubmittedWithdrawClassrunNotifyApproverEvent : BaseTodoEvent<SubmittedWithdrawClassrunNotifyApproverPayload>
    {
        public SubmittedWithdrawClassrunNotifyApproverEvent(Guid createBy, SubmittedWithdrawClassrunNotifyApproverPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:learner-submit-withdraw:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Withdrawal Pending Confirmation";
            Template = "LearnerSubmitWithdrawClassrunPending";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
