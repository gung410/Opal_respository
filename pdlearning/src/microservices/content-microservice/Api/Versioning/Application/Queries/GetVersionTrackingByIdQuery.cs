using System;
using Microservice.Content.Versioning.Application.Model;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Versioning.Application.Queries
{
    public class GetVersionTrackingByIdQuery : BaseThunderQuery<VersionTrackingModel>
    {
        public Guid UserId { get; set; }

        public Guid VersionId { get; set; }
    }
}
