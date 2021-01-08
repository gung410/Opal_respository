using System;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetSessionsByClassRunIdQuery : BaseThunderQuery<PagedResultDto<SessionModel>>
    {
        public Guid ClassRunId { get; set; }

        public SearchSessionType SearchType { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
