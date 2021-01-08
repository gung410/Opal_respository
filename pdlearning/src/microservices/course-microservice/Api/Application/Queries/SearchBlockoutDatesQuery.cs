using System;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class SearchBlockoutDatesQuery : BaseThunderQuery<PagedResultDto<BlockoutDateModel>>
    {
        public string SearchText { get; set; }

        public Guid CoursePlanningCycleId { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }

        public CommonFilter Filter { get; set; }
    }
}
