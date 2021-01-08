using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class AssignmentFeedbackNotifyCFEvent : BaseTodoEvent<AssignmentFeedbackNotifyCFPayload>
    {
        public AssignmentFeedbackNotifyCFEvent(Guid createBy, AssignmentFeedbackNotifyCFPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:assignment-feedback-to-course-facilitator:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Assignment Feedback Received";
            Template = "AssignmentFeedbackToCF";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
