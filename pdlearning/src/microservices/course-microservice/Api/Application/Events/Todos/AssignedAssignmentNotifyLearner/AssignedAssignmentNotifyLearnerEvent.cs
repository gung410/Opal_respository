using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class AssignedAssignmentNotifyLearnerEvent : BaseTodoEvent<AssignedAssignmentNotifyLearnerPayload>
    {
        public AssignedAssignmentNotifyLearnerEvent(Guid createBy, AssignedAssignmentNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:assign-assignment:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Assignment received";
            Template = "AssignmentAssignedToLearner";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
