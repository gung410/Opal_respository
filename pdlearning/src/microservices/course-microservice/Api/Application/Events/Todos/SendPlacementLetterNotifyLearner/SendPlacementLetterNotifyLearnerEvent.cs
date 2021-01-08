using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class SendPlacementLetterNotifyLearnerEvent : BaseTodoEvent<SendPlacementLetterNotifyLearnerPayload>
    {
        public SendPlacementLetterNotifyLearnerEvent(Guid createBy, SendPlacementLetterNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:send-placement-letter:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Placement Letter";
            Template = "LearnerRegistrationClassRunConfirm";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
