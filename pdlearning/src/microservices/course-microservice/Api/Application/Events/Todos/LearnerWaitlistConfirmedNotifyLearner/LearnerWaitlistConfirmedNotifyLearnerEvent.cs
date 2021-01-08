using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class LearnerWaitlistConfirmedNotifyLearnerEvent : BaseTodoEvent<LearnerWaitlistConfirmedNotifyLearnerPayload>
    {
        public LearnerWaitlistConfirmedNotifyLearnerEvent(Guid createBy, LearnerWaitlistConfirmedNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:registration-waitlist:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Application on Waitlist";
            Template = "LearnerRegistrationWaitlistConfirmed";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
