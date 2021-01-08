using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class ManualChangedClassLearnerNotifyLearnerEvent : BaseTodoEvent<ManualChangedClassLearnerNotifyLearnerPayload>
    {
        public ManualChangedClassLearnerNotifyLearnerEvent(Guid createBy, ManualChangedClassLearnerNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:change-class-learner-manually:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Change Class Announcement";
            Template = "CAChangeClassLearnerManually";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
