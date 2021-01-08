using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Application.RequestDtos;
using Microservice.Badge.Domain.ValueObjects;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Badge.Application.Services
{
    public interface IBadgeService
    {
        Task<List<BadgeModel>> GetBadges();

        Task<BadgeWithCriteriaModel<ActiveContributorsBadgeCriteria>> GetActiveContributorBadge();

        Task<List<UserBadgeModel>> GetGeneralBadgesByUserId(Guid userId);

        Task<PagedResultDto<UserBadgeModel>> GetAwardedBadgesByUserId(Guid userId, PagedResultRequestDto request);

        Task<PagedResultDto<UserBadgeModel>> GetCommunityBadgesByUserId(Guid userId, PagedResultRequestDto request);

        Task<PagedResultDto<UserBadgeModel>> GetCommunityUserBadgesByCommunityId(Guid communityId, PagedResultRequestDto request);

        Task<PagedResultDto<UserRewardStatisticModel>> SearchTopBadgeUser(SearchTopBadgeUserRequest request);

        Task Award(RewardUserBadgesRequest request);

        Task SaveActiveContributorBadgeCriteria(SaveActiveContributorBadgeCriteriaRequest request);

        Task<PagedResultDto<UserBadgeModel>> GetAwardedBadgesByIds(Guid userId, GetAwardedBadgesByIdsRequest request);
    }
}
