using System;
using Microservice.Form.Versioning.Application.Model;
using Microservice.Form.Versioning.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Versioning.Application.Queries
{
    public class GetVersionTrackingByObjectIdQuery : BaseThunderQuery<PagedResultDto<VersionTrackingModel>>
    {
        public Guid UserId { get; set; }

        public GetVersionTrackingByObjectIdRequest Request { get; set; }
    }
}
