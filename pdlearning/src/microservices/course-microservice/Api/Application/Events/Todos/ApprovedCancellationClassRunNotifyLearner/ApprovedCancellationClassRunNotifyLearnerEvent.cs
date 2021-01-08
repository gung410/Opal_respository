using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class ApprovedCancellationClassRunNotifyLearnerEvent : BaseTodoEvent<ApprovedCancellationClassRunNotifyLearnerPayload>
    {
        public ApprovedCancellationClassRunNotifyLearnerEvent(Guid createBy, ApprovedCancellationClassRunNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:classrun-cancel-approved:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Class Cancelled";
            Template = "CAOApprovedClassrunCancelation";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
