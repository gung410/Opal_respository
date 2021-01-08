using System;
using Microservice.LnaForm.Versioning.Application.Model;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Versioning.Application.Queries
{
    public class GetVersionTrackingByIdQuery : BaseThunderQuery<VersionTrackingModel>
    {
        public Guid UserId { get; set; }

        public Guid VersionId { get; set; }
    }
}
