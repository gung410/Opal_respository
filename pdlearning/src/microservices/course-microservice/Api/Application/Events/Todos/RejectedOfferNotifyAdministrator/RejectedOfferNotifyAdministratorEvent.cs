using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class RejectedOfferNotifyAdministratorEvent : BaseTodoEvent<RejectedOfferNotifyAdministratorPayload>
    {
        public RejectedOfferNotifyAdministratorEvent(Guid createBy, RejectedOfferNotifyAdministratorPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:offer-rejected:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Offer Declined";
            Template = "LearnerDeclinePlaceOffer";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
