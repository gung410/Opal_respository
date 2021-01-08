using System;
using Microservice.LnaForm.Versioning.Application.Model;
using Microservice.LnaForm.Versioning.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Versioning.Application.Queries
{
    public class GetVersionTrackingByObjectIdQuery : BaseThunderQuery<PagedResultDto<VersionTrackingModel>>
    {
        public Guid UserId { get; set; }

        public GetVersionTrackingByObjectIdRequest Request { get; set; }
    }
}
