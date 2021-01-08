using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class VerifiedCourseInPlanningCycleNotifyOwnerEvent : BaseTodoEvent<VerifiedCourseInPlanningCycleNotifyOwnerPayload>
    {
        public VerifiedCourseInPlanningCycleNotifyOwnerEvent(Guid createBy, VerifiedCourseInPlanningCycleNotifyOwnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:course-in-planning-cycle-verified:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Course Verified";
            Template = "CourseInPlanningCycleVerified";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
