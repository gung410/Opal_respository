using System;
using System.Collections.Generic;
using Microservice.Form.Versioning.Application.Model;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Versioning.Application.Queries
{
    public class GetRevertableVersionsQuery : BaseThunderQuery<List<VersionTrackingModel>>
    {
        public Guid UserId { get; set; }

        public Guid OriginalObjectId { get; set; }
    }
}
