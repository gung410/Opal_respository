using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Badge.Application.Commands;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Application.Queries;
using Microservice.Badge.Application.RequestDtos;
using Microservice.Badge.Domain.ValueObjects;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Application.Services
{
    /// <summary>
    /// This service work as forwarder for each request in controller. It different with business logic.
    /// </summary>
    public class BadgeService : BaseApplicationService, IBadgeService
    {
        // todo: consider to remove it and call command/query directly in controller
        public BadgeService(IThunderCqrs thunderCqrs) : base(thunderCqrs)
        {
        }

        public Task<List<BadgeModel>> GetBadges()
        {
            return ThunderCqrs.SendQuery(new GetBadgesQuery());
        }

        public Task<BadgeWithCriteriaModel<ActiveContributorsBadgeCriteria>> GetActiveContributorBadge()
        {
            return ThunderCqrs.SendQuery(new GetActiveContributorBadgeQuery());
        }

        public Task<List<UserBadgeModel>> GetGeneralBadgesByUserId(Guid userId)
        {
            return ThunderCqrs.SendQuery(new GetGeneralBadgesByUserIdQuery(userId));
        }

        public Task<PagedResultDto<UserBadgeModel>> GetAwardedBadgesByUserId(Guid userId, PagedResultRequestDto request)
        {
            GetAwardedBadgesByUserIdQuery query = new(userId, InitPagingRequestDto(request));
            return ThunderCqrs.SendQuery(query);
        }

        public Task<PagedResultDto<UserBadgeModel>> GetAwardedBadgesByIds(Guid userId, GetAwardedBadgesByIdsRequest request)
        {
            return ThunderCqrs.SendQuery(
                new GetAwardedBadgesByIdsQuery
                {
                    UserId = userId,
                    Data = request.Data,
                    PageInfo = InitPagingRequestDto(request)
                });
        }

        public Task<PagedResultDto<UserBadgeModel>> GetCommunityBadgesByUserId(Guid userId, PagedResultRequestDto request)
        {
            GetCommunityUserBadgesByUserIdQuery query = new(userId, InitPagingRequestDto(request));
            return ThunderCqrs.SendQuery(query);
        }

        public Task<PagedResultDto<UserBadgeModel>> GetCommunityUserBadgesByCommunityId(Guid communityId, PagedResultRequestDto request)
        {
            GetCommunityUserBadgesByCommunityIdQuery query = new(communityId, InitPagingRequestDto(request));
            return ThunderCqrs.SendQuery(query);
        }

        public Task<PagedResultDto<UserRewardStatisticModel>> SearchTopBadgeUser(SearchTopBadgeUserRequest request)
        {
            return ThunderCqrs.SendQuery(
                new SearchTopBadgeUserQuery
                {
                    BadgeId = request.BadgeId,
                    PageInfo = InitPagingRequestDto(request),
                    SearchText = request.SearchText
                });
        }

        public Task SaveActiveContributorBadgeCriteria(SaveActiveContributorBadgeCriteriaRequest request)
        {
            return ThunderCqrs.SendCommand(
                new SaveActiveContributorBadgeCriteriaCommand
                {
                    Criteria = request.Criteria
                });
        }

        public Task Award(RewardUserBadgesRequest request)
        {
            return ThunderCqrs.SendCommand(new AwardBadgeCommand { BadgeId = request.BadgeId, UserIds = request.UserIds });
        }
    }
}
