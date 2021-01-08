using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class CreatedCourseNotifyApproverEvent : BaseTodoEvent<CreatedCourseNotifyApproverPayload>
    {
        public CreatedCourseNotifyApproverEvent(Guid createBy, CreatedCourseNotifyApproverPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:course-approval:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - New course pending approval";
            Template = "NewCourseRequestApproval";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
