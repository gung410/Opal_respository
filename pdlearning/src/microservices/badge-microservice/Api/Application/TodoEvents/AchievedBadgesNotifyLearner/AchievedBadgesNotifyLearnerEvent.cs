using System;
using System.Collections.Generic;

namespace Microservice.Badge.Application.TodoEvents
{
    public class AchievedBadgesNotifyLearnerEvent : BaseTodoEvent<AchievedBadgesNotifyLearnerPayload>
    {
         public AchievedBadgesNotifyLearnerEvent(Guid createBy, AchievedBadgesNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:achieved-badges-notify-learner:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Badge Awarded";
            Template = "LearnerAchievedBadges";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

         public List<Guid> AssignedToIds { get; }
    }
}
