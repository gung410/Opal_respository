using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class SearchBlockoutDatesRequest : PagedResultRequestDto
    {
        public string SearchText { get; set; }

        public Guid CoursePlanningCycleId { get; set; }

        public CommonFilter Filter { get; set; }
    }
}
