using System;
using Microservice.Content.Versioning.Application.Dtos;
using Microservice.Content.Versioning.Application.Model;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Versioning.Application.Queries
{
    public class GetVersionTrackingByObjectIdQuery : BaseThunderQuery<PagedResultDto<VersionTrackingModel>>
    {
        public Guid UserId { get; set; }

        public GetVersionTrackingByObjectIdRequest Request { get; set; }
    }
}
