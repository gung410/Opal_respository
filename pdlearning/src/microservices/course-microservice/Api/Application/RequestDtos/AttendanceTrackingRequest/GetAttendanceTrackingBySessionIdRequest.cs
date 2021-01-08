using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetAttendanceTrackingBySessionIdRequest : PagedResultRequestDto
    {
        public Guid SessionId { get; set; }

        public string SearchText { get; set; }

        public CommonFilter Filter { get; set; }
    }
}
