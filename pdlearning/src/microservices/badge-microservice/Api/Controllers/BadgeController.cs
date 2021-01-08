using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Application.RequestDtos;
using Microservice.Badge.Application.Services;
using Microservice.Badge.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Controllers
{
    // todo: consider to call command/query directly
    [Route("api/badges")]
    public class BadgeController : ApplicationApiController
    {
        private readonly IBadgeService _badgeService;
        private readonly IThunderCqrs _thunderCqrs;

        public BadgeController(
            IUserContext userContext,
            IBadgeService badgeService,
            IThunderCqrs thunderCqrs) : base(userContext)
        {
            _badgeService = badgeService;
            _thunderCqrs = thunderCqrs;
        }

        [HttpGet("getInfo")]
        public async Task<List<BadgeModel>> GetBadges()
        {
            return await _badgeService.GetBadges();
        }

        [HttpGet("activeContributorBadge")]
        public async Task<BadgeWithCriteriaModel<ActiveContributorsBadgeCriteria>> GetActiveContributorBadge()
        {
            return await _badgeService.GetActiveContributorBadge();
        }

        [HttpGet("currentUser/GetAwardedBadges")]
        public async Task<PagedResultDto<UserBadgeModel>> GetAwardedBadges([FromQuery] PagedResultRequestDto request)
        {
            return await _badgeService.GetAwardedBadgesByUserId(CurrentUserId, request);
        }

        [HttpGet("currentUser/GetGeneralBadges")]
        public async Task<List<UserBadgeModel>> GetGeneralBadgesForCurrentUser()
        {
            return await _badgeService.GetGeneralBadgesByUserId(CurrentUserId);
        }

        [HttpGet("currentUser/GetCommunityBadges")]
        public async Task<PagedResultDto<UserBadgeModel>> GetCommunityBadgesForCurrentUser([FromQuery] PagedResultRequestDto request)
        {
            return await _badgeService.GetCommunityBadgesByUserId(CurrentUserId, request);
        }

        [HttpPost("currentUser/GetAwardedBadgesByIds")]
        public async Task<PagedResultDto<UserBadgeModel>> GetAwardedBadgesByIds([FromBody] GetAwardedBadgesByIdsRequest request)
        {
            return await _badgeService.GetAwardedBadgesByIds(CurrentUserId, request);
        }

        [HttpGet("{communityId:guid}/GetCommunityBadges")]
        public async Task<PagedResultDto<UserBadgeModel>> GetCommunityUserBadges(Guid communityId, [FromQuery] PagedResultRequestDto request)
        {
            return await _badgeService.GetCommunityUserBadgesByCommunityId(communityId, request);
        }

        [HttpPost("searchTopBadgeUserStatistics")]
        public async Task<PagedResultDto<UserRewardStatisticModel>> SearchTopBadgeUserStatistics([FromBody] SearchTopBadgeUserRequest request)
        {
            return await _badgeService.SearchTopBadgeUser(request);
        }

        [HttpPost("getUserBadgesByUserIds")]
        public Task<List<UserRewardStatisticModel>> GetUserBadgesByUserIds()
        {
            return null;
        }

        [HttpPost("rewards")]
        public async Task Award([FromBody] RewardUserBadgesRequest request)
        {
            await _badgeService.Award(request);
        }

        [HttpPost("saveActiveContributorCriteria")]
        public async Task SaveActiveContributorBadgeCriteria([FromBody] SaveActiveContributorBadgeCriteriaRequest request)
        {
            await _badgeService.SaveActiveContributorBadgeCriteria(request);
        }
    }
}
