using System;
using System.Collections.Generic;
using Microservice.Content.Versioning.Application.Model;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Versioning.Application.Queries
{
    public class GetRevertableVersionsQuery : BaseThunderQuery<List<VersionTrackingModel>>
    {
        public Guid UserId { get; set; }

        public Guid OriginalObjectId { get; set; }
    }
}
