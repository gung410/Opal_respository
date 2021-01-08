using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class RemindConfirmRegistrationsNotifyEvent : BaseTodoEvent<RemindConfirmRegistrationsNotifyEventPayload>
    {
        public RemindConfirmRegistrationsNotifyEvent(Guid createBy, RemindConfirmRegistrationsNotifyEventPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:registration-confirmation-reminder:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Registration Confirmation Reminder";
            Template = "RegistrationConfirmationReminder";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
