using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class TakeAttendanceNotifyLearnerEvent : BaseTodoEvent<TakeAttendanceNotifyLearnerPayload>
    {
        public TakeAttendanceNotifyLearnerEvent(TakeAttendanceNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:attendance-status-remind:{Guid.NewGuid()}";
            Subject = "OPAL2.0 – Taking attendance reminder";
            Template = "TakeAttendanceRemind";
            Payload = payload;
            AssignedToIds = assignedToIds;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
