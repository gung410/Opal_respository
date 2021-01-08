using System;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetCommunityEventsByCommunityIdQuery : BaseThunderQuery<PagedResultDto<CommunityEventModel>>
    {
        public GetCommunityEventsByCommunityIdRequest Request { get; set; }

        public Guid UserId { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
