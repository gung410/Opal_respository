using System;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class SearchAnnouncementQuery : BaseThunderQuery<PagedResultDto<AnnouncementModel>>
    {
        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }

        public CommonFilter Filter { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
