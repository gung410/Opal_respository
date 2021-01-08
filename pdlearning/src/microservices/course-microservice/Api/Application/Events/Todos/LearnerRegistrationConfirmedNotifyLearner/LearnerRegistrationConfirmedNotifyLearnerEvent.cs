using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class LearnerRegistrationConfirmedNotifyLearnerEvent : BaseTodoEvent<LearnerRegistrationConfirmedNotifyLearnerPayload>
    {
        public LearnerRegistrationConfirmedNotifyLearnerEvent(Guid createBy, LearnerRegistrationConfirmedNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:registration-confirmed:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Course Registration Confirmed";
            Template = "LearnerRegistrationClassRunConfirmed";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
