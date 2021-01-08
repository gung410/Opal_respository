using System;
using System.Collections.Generic;
using Microservice.LnaForm.Versioning.Application.Model;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Versioning.Application.Queries
{
    public class GetRevertableVersionsQuery : BaseThunderQuery<List<VersionTrackingModel>>
    {
        public Guid UserId { get; set; }

        public Guid OriginalObjectId { get; set; }
    }
}
