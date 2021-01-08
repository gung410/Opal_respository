using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class AssignmentDueDateNotifyLearnerEvent : BaseTodoEvent<AssignmentDueDateNotifyLearnerPayload>
    {
        public AssignmentDueDateNotifyLearnerEvent(Guid createBy, AssignmentDueDateNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:assignment-duedate:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Assignment due date";
            Template = "AssignmentDueDate";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
