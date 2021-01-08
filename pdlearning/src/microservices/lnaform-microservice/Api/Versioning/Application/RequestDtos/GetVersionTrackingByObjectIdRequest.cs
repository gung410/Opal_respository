using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.LnaForm.Versioning.Application.RequestDtos
{
    public class GetVersionTrackingByObjectIdRequest
    {
        public Guid OriginalObjectId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
