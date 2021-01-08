using System;
using Microservice.Form.Versioning.Application.Model;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Versioning.Application.Queries
{
    public class GetVersionTrackingByIdQuery : BaseThunderQuery<VersionTrackingModel>
    {
        public Guid UserId { get; set; }

        public Guid VersionId { get; set; }
    }
}
