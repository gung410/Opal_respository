using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class LearnerRegistrationRejectedNotifyLearnerEvent : BaseTodoEvent<LearnerRegistrationRejectedNotifyLearnerPayload>
    {
        public LearnerRegistrationRejectedNotifyLearnerEvent(Guid createBy, LearnerRegistrationRejectedNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:api:registration-rejected:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Application for Course Rejected";
            Template = "LearnerRegistrationClassRunRejected";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
