using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class CoursePlanningCycleStartedNotifyCCCEvent : BaseTodoEvent<CoursePlanningCycleStartedNotifyCCCPayload>
    {
        public CoursePlanningCycleStartedNotifyCCCEvent(CoursePlanningCycleStartedNotifyCCCPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:course-planning-cycle-started:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Course Planning Cycle";
            Template = "CoursePlanningCycleStarted";
            Payload = payload;
            AssignedToIds = assignedToIds;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
