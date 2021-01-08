using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class LearnerRegistrationNotifyAdministratorEvent : BaseTodoEvent<LearnerRegistrationNotifyAdministratorPayload>
    {
        public LearnerRegistrationNotifyAdministratorEvent(Guid createBy, LearnerRegistrationNotifyAdministratorPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:classrun-register:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - New Registration Pending Confirmation";
            Template = "LearnerRegistrationClassRun";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
