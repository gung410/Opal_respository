using System;
using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ.Contract;

namespace Microservice.Course.Application.Events.Todos
{
    public class AbsenceNotifyLearnerEvent : BaseTodoEvent<AbsenceNotifyLearnerPayload>
    {
        public AbsenceNotifyLearnerEvent(AbsenceNotifyLearnerPayload payload, List<Guid> assignedToIds, ReminderByDto reminderByConditions)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:attendance-status:{Guid.NewGuid()}";
            Subject = "OPAL2.0 – Attendance status update";
            Template = "AttendanceStatusUpdate";
            Payload = payload;
            AssignedToIds = assignedToIds;
            ReminderByConditions = reminderByConditions;
        }

        public List<Guid> AssignedToIds { get; }

        public ReminderByDto ReminderByConditions { get; }
    }
}
