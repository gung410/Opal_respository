using System;
using Microservice.Course.Application.Enums;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetClassRunsByCourseIdRequest : PagedResultRequestDto
    {
        public Guid CourseId { get; set; }

        public SearchClassRunType SearchType { get; set; }

        public bool? LoadHasContentInfo { get; set; }

        public string SearchText { get; set; }

        public bool? NotStarted { get; set; }

        public bool? NotEnded { get; set; }

        public CommonFilter Filter { get; set; }
    }
}
