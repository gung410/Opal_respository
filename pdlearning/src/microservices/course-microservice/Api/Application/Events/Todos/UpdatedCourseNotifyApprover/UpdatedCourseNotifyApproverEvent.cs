using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class UpdatedCourseNotifyApproverEvent : BaseTodoEvent<UpdatedCourseNotifyApproverPayload>
    {
        public UpdatedCourseNotifyApproverEvent(Guid createBy, UpdatedCourseNotifyApproverPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:course-update-approval:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Course pending approval";
            Template = "CourseUpdatedRequestApproval";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
