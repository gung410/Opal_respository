using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class ChangeClassLearnerNotifyAdministratorEvent : BaseTodoEvent<ChangeClassLearnerNotifyAdministratorPayload>
    {
        public ChangeClassLearnerNotifyAdministratorEvent(Guid createBy, ChangeClassLearnerNotifyAdministratorPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:change-class-pending:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Change Class Pending Confirmation";
            Template = "LearnerChangeClassPending";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
