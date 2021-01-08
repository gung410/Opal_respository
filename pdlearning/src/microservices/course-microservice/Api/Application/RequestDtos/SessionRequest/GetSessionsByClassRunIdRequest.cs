using System;
using Microservice.Course.Application.Enums;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetSessionsBySessionIdRequest : PagedResultRequestDto
    {
        public Guid ClassRunId { get; set; }

        public SearchSessionType SearchType { get; set; }
    }
}
