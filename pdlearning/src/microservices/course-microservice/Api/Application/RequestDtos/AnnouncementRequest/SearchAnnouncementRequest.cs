using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class SearchAnnouncementRequest : PagedResultRequestDto
    {
        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }

        public CommonFilter Filter { get; set; }
    }
}
