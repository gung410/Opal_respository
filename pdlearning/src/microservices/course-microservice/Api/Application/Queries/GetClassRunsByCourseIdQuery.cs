using System;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetClassRunsByCourseIdQuery : BaseThunderQuery<PagedResultDto<ClassRunModel>>
    {
        public Guid CourseId { get; set; }

        public SearchClassRunType SearchType { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }

        public bool? LoadHasContentInfo { get; set; }

        public string SearchText { get; set; }

        public bool? NotStarted { get; set; }

        public bool? NotEnded { get; set; }

        public CommonFilter Filter { get; set; }
    }
}
