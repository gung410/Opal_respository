using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class RejectedCourseNotifyOwnerEvent : BaseTodoEvent<RejectedCourseNotifyOwnerPayload>
    {
        public RejectedCourseNotifyOwnerEvent(Guid createBy, RejectedCourseNotifyOwnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:course-approval-rejected:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Course Rejected";
            Template = "CourseRequestApprovalRejected";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
