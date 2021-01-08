using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class RejectedRegistrationNotifyLearnerEvent : BaseTodoEvent<RejectedRegistrationNotifyLearnerPayload>
    {
        public RejectedRegistrationNotifyLearnerEvent(Guid createBy, RejectedRegistrationNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:registration-rejected:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Registration Rejected";
            Template = "CourseAdminRejectLearnerRegistration";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
