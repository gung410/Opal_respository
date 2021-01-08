using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class SendOfferNotifyLearnerEvent : BaseTodoEvent<SendOfferNotifyLearnerPayload>
    {
        public SendOfferNotifyLearnerEvent(Guid createBy, SendOfferNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:send-offer:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Placement Offer";
            Template = "CourseAdminSendOfferWaitListToRegistration";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
