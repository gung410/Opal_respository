using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class ChangeLearningMethodNotifyInvolvedUserEvent : BaseTodoEvent<ChangeLearningMethodNotifyInvolvedUserPayload>
    {
        public ChangeLearningMethodNotifyInvolvedUserEvent(List<Guid> assignedToIds)
        {
            AssignedToIds = assignedToIds;
        }

        public List<Guid> AssignedToIds { get; }

        public static ChangeLearningMethodNotifyInvolvedUserEvent CreateForChangeLearningMethodToOnline(
            Guid createBy,
            ChangeLearningMethodNotifyInvolvedUserPayload payload,
            List<Guid> assignedToIds)
        {
            return new ChangeLearningMethodNotifyInvolvedUserEvent(assignedToIds)
            {
                TaskURI = $"urn:schemas:conexus:dls:course-api:change-learning-method-notify-involved-user:{Guid.NewGuid()}",
                Subject = $"OPAL2.0 - Update of Learning Mode for {payload.CourseName}",
                Template = "ChangeLearningMethodToOnlineNotifyInvolvedUser",
                Payload = payload,
                CreatedBy = createBy,
            };
        }

        public static ChangeLearningMethodNotifyInvolvedUserEvent CreateForChangeLearningMethodToOffline(
            Guid createBy,
            ChangeLearningMethodNotifyInvolvedUserPayload payload,
            List<Guid> assignedToIds)
        {
            return new ChangeLearningMethodNotifyInvolvedUserEvent(assignedToIds)
            {
                TaskURI = $"urn:schemas:conexus:dls:course-api:change-learning-method-notify-involved-user:{Guid.NewGuid()}",
                Subject = $"OPAL2.0 - Update of Learning Mode for {payload.CourseName}",
                Template = "ChangeLearningMethodToOfflineNotifyInvolvedUser",
                Payload = payload,
                CreatedBy = createBy,
            };
        }
    }
}
