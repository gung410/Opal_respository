using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class PublishedClassRunNotifyCollaboratorEvent : BaseTodoEvent<PublishedClassRunNotifyCollaboratorPayload>
    {
        public PublishedClassRunNotifyCollaboratorEvent(Guid createBy, PublishedClassRunNotifyCollaboratorPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:classrun-published:{Guid.NewGuid()}";
            Subject = "New Classrun Published";
            Template = "ClassrunForNonMicroPublishedNotifyCollaborator";
            Payload = payload;
            CreatedBy = createBy;
            AssignedToIds = assignedToIds;
        }

        public List<Guid> AssignedToIds { get; }
    }
}
