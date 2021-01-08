using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class NotifyLearnerRegistrationUnsuccessfulEvent : BaseTodoEvent<NotifyLearnerRegistrationUnsuccessfulPayload>
    {
        public NotifyLearnerRegistrationUnsuccessfulEvent(Guid createBy, NotifyLearnerRegistrationUnsuccessfulPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:change-learning-method-notify-involved-user:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Registration Unsuccessful";
            Template = "NotifyLearnerRegistrationUnsuccessful";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
