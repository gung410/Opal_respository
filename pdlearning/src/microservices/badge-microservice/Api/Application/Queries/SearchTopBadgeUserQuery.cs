using System;
using Microservice.Badge.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Application.Queries
{
    public class SearchTopBadgeUserQuery : BaseThunderQuery<PagedResultDto<UserRewardStatisticModel>>
    {
        public Guid BadgeId { get; set; }

        public string SearchText { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
