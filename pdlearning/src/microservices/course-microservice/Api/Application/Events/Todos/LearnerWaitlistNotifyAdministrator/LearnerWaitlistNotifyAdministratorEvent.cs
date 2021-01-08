using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class LearnerWaitlistNotifyAdministratorEvent : BaseTodoEvent<LearnerWaitlistNotifyAdministratorPayload>
    {
        public LearnerWaitlistNotifyAdministratorEvent(Guid createBy, LearnerWaitlistNotifyAdministratorPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:classrun-learner-waitlist:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - New Registration on Waitlist";
            Template = "LearnerInWaitListClassRun";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
