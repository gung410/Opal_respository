using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class ApprovedCourseInPlanningCycleNotifyCPCEvent : BaseTodoEvent<ApprovedCourseInPlanningCycleNotifyCPCPayload>
    {
        public ApprovedCourseInPlanningCycleNotifyCPCEvent(Guid createBy, ApprovedCourseInPlanningCycleNotifyCPCPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:course-in-planning-cycle-pending:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Course pending verification";
            Template = "CourseInPlanningCyclePending";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
