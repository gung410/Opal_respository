using System;

namespace Microservice.Content.Application.RequestDtos
{
    public class GetContentByVersionTrackingIdRequestDto
    {
        public Guid VersionTrackingId { get; set; }

        public Guid UserId { get; set; }
    }
}
