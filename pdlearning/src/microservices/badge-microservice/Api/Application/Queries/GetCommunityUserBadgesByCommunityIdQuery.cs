using System;
using Microservice.Badge.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Application.Queries
{
    public class GetCommunityUserBadgesByCommunityIdQuery : BaseThunderQuery<PagedResultDto<UserBadgeModel>>
    {
        public GetCommunityUserBadgesByCommunityIdQuery(Guid communityId, PagedResultRequestDto pageInfo)
        {
            CommunityId = communityId;
            PageInfo = pageInfo;
        }

        public Guid CommunityId { get; init; }

        public PagedResultRequestDto PageInfo { get; init; }
    }
}
