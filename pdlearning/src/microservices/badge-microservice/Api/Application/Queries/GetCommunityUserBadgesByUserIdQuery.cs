using System;
using Microservice.Badge.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Application.Queries
{
    public class GetCommunityUserBadgesByUserIdQuery : BaseThunderQuery<PagedResultDto<UserBadgeModel>>
    {
        public GetCommunityUserBadgesByUserIdQuery(Guid userId, PagedResultRequestDto pageInfo)
        {
            UserId = userId;
            PageInfo = pageInfo;
        }

        public Guid UserId { get; init; }

        public PagedResultRequestDto PageInfo { get; init; }
    }
}
