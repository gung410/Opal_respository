using System;
using System.Collections.Generic;

namespace Microservice.Learner.Application.Events.Todo
{
    public class ArchivedCourseNotificationEvent : BaseTodoEvent<ArchivedCourseNotificationPayload>
    {
        public ArchivedCourseNotificationEvent(Guid createBy, ArchivedCourseNotificationPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:learner-api:notify-archived-course:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Course Archived";
            Template = "CourseArchived";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
