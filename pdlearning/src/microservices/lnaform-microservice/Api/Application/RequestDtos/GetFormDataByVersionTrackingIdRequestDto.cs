using System;

namespace Microservice.LnaForm.Application.RequestDtos
{
    public class GetFormDataByVersionTrackingIdRequestDto
    {
        public Guid VersionTrackingId { get; set; }

        public Guid UserId { get; set; }
    }
}
