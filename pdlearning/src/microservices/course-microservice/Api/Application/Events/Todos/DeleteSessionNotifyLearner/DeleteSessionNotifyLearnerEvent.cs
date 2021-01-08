using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class DeleteSessionNotifyLearnerEvent : BaseTodoEvent<DeleteSessionNotifyLearnerPayload>
    {
        public DeleteSessionNotifyLearnerEvent(Guid createBy, DeleteSessionNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:session-cancelled:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Session Cancelled";
            Template = "SessionCancelledByCA";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
