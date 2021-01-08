using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class AssessmentAnswerCompletedNotifyLearnerEvent : BaseTodoEvent<AssessmentAnswerCompletedNotifyLearnerPayload>
    {
        public AssessmentAnswerCompletedNotifyLearnerEvent(Guid createBy, AssessmentAnswerCompletedNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:assessment-answer-completed:{Guid.NewGuid()}";
            Subject = string.Empty;
            Template = "AssessmentAnswerCompletedNotifyLearner";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
