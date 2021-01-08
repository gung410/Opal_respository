using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class ApprovedRescheduleClassRunNotifyLearnerEvent : BaseTodoEvent<ApprovedRescheduleClassRunNotifyLearnerPayload>
    {
        public ApprovedRescheduleClassRunNotifyLearnerEvent(Guid createBy, ApprovedRescheduleClassRunNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:classrun-reschedule-approved:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Class Rescheduled";
            Template = "CAOApprovedClassrunReschedulation";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
