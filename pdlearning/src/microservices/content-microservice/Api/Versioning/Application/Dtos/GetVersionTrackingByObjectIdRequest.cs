using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Content.Versioning.Application.Dtos
{
    public class GetVersionTrackingByObjectIdRequest
    {
        public Guid OriginalObjectId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
