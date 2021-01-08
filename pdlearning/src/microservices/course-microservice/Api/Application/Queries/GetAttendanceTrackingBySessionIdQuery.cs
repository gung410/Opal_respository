using System;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetAttendanceTrackingBySessionIdQuery : BaseThunderQuery<PagedResultDto<AttendanceTrackingModel>>
    {
        public Guid Id { get; set; }

        public string SearchText { get; set; }

        public CommonFilter Filter { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
