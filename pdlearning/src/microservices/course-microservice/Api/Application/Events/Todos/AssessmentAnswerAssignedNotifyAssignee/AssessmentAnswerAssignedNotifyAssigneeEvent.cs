using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class AssessmentAnswerAssignedNotifyAssigneeEvent : BaseTodoEvent<AssessmentAnswerAssignedNotifyAssigneePayload>
    {
        public AssessmentAnswerAssignedNotifyAssigneeEvent(Guid createBy, AssessmentAnswerAssignedNotifyAssigneePayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:assessment-answer-assigned:{Guid.NewGuid()}";
            Subject = string.Empty;
            Template = "AssessmentAnswerAssignedNotifyAssignee";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
