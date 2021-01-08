using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class SubmittedCourseContentNotifyApproverEvent : BaseTodoEvent<SubmittedCourseContentNotifyApproverPayload>
    {
        public SubmittedCourseContentNotifyApproverEvent(Guid createBy, SubmittedCourseContentNotifyApproverPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:course-content-approval:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Course Content pending approval";
            Template = "CourseContentPendingApproval";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
