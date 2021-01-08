using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class ApprovedCourseMLUNotifyCollaboratorEvent : BaseTodoEvent<ApprovedCourseMLUNotifyCollaboratorPayload>
    {
        public ApprovedCourseMLUNotifyCollaboratorEvent(Guid createBy, ApprovedCourseMLUNotifyCollaboratorPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:course-published-for-collaborator:{Guid.NewGuid()}";
            Subject = "New MLU Approved";
            Template = "CourseMLUApprovedNotifyCollaborator";
            Payload = payload;
            CreatedBy = createBy;
            AssignedToIds = assignedToIds;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
