using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class LearnerRegistrationNotifyApproverEvent : BaseTodoEvent<LearnerRegistrationNotifyApproverPayload>
    {
        public LearnerRegistrationNotifyApproverEvent(Guid createBy, LearnerRegistrationNotifyApproverPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:learner-register-class-run:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - New Registration Pending Approval";
            Template = "LearnerPendingApprovalClassRun";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
