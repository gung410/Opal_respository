using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class RejectedCourseInPlanningCycleNotifyOwnerEvent : BaseTodoEvent<RejectedCourseInPlanningCycleNotifyOwnerPayload>
    {
        public RejectedCourseInPlanningCycleNotifyOwnerEvent(Guid createBy, RejectedCourseInPlanningCycleNotifyOwnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:course-in-planning-cycle-rejected:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Course Rejected";
            Template = "CourseInPlanningCycleRejected";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
