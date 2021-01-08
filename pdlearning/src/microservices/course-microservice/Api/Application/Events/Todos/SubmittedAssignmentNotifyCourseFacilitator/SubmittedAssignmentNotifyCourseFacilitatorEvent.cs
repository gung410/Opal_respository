using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class SubmittedAssignmentNotifyCourseFacilitatorEvent : BaseTodoEvent<SubmittedAssignmentNotifyCourseFacilitatorPayload>
    {
        public SubmittedAssignmentNotifyCourseFacilitatorEvent(Guid createBy, SubmittedAssignmentNotifyCourseFacilitatorPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:assignment:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Assignment received";
            Template = "LearnerSubmitAssignment";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
