using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class AcceptOfferNotifyAdministratorEvent : BaseTodoEvent<AcceptOfferNotifyAdministratorPayload>
    {
        public AcceptOfferNotifyAdministratorEvent(Guid createBy, AcceptOfferNotifyAdministratorPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:accept-offer:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Offer Accepted";
            Template = "LearnerAcceptPlaceOffer";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
