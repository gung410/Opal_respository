using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries
{
    public class GetAllCollaboratorsIdQuery : BaseThunderQuery<List<Guid>>
    {
        public GetAllCollaboratorsIdQuery(Guid originalObjectId)
        {
            OriginalObjectId = originalObjectId;
        }

        public Guid OriginalObjectId { get; set; }
    }
}
