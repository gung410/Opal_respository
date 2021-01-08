using System;
using Microservice.Calendar.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetCalendarAccessSharingsQuery : BaseThunderQuery<PagedResultDto<UserAccessSharingModel>>
    {
        public Guid OwnerId { get; set; }

        public PagedResultRequestDto PagingRequest { get; set; }
    }
}
