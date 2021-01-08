using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class ChangeClassSubmittedByLearnerNotifyApproverEvent : BaseTodoEvent<ChangeClassSubmittedByLearnerNotifyApproverPayload>
    {
        public ChangeClassSubmittedByLearnerNotifyApproverEvent(Guid createBy, ChangeClassSubmittedByLearnerNotifyApproverPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:learner-submit-change-class:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Change Class Pending Confirmation";
            Template = "LearnerSubmittedChangeClassPending";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
