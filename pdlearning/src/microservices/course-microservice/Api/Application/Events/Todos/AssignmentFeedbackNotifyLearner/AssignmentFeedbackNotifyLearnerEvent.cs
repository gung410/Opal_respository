using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class AssignmentFeedbackNotifyLearnerEvent : BaseTodoEvent<AssignmentFeedbackNotifyLearnerPayload>
    {
        public AssignmentFeedbackNotifyLearnerEvent(Guid createBy, AssignmentFeedbackNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:assignment-feedback:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Assignment Feedback Received";
            Template = "AssignmentFeedback";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
