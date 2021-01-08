using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class ApprovedCourseNotifyOwnerEvent : BaseTodoEvent<ApprovedCourseNotifyOwnerPayload>
    {
        public ApprovedCourseNotifyOwnerEvent(Guid createBy, ApprovedCourseNotifyOwnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:course-approved:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Course Approved";
            Template = "CourseRequestApprovalApproved";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
