using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class AssignmentExtendedNotifyLearnerEvent : BaseTodoEvent<AssignmentExtendedNotifyLearnerPayload>
    {
        public AssignmentExtendedNotifyLearnerEvent(Guid createBy, AssignmentExtendedNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:assignment-extended-duedate:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Assignment Extended Due Date";
            Template = "AssignmentExtendedDueDate";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
