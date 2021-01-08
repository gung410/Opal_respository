using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class ManualWithdrawnLearnerNotifyLearnerEvent : BaseTodoEvent<ManualWithdrawnLearnerNotifyLearnerPayload>
    {
        public ManualWithdrawnLearnerNotifyLearnerEvent(Guid createBy, ManualWithdrawnLearnerNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:withdraw-learner-manually:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Withdrawal Announcement";
            Template = "CAWithdrawLearnerManually";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
