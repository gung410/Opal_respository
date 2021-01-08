using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Badge.Application.TodoEvents;
using Microservice.Badge.Domain.Constants;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using Microservice.Badge.Settings;
using MongoDB.Driver;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Application.BusinessLogic
{
    public class ProcessBadgeLogic : ApplicationService, IProcessBadgeLogic
    {
        private readonly BadgeDbContext _dbContext;
        private readonly IProcessTopBadgeQualifiedUserLogic _processTopBadgeQualifiedUserLogic;
        private readonly IGeneralBadgeCriteriaResolverLogic<LifeLongBadgeCriteria, Guid> _lifeLongCriteriaResolverLogic;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly IThunderCqrs _thunderCqrs;

        private int _pageSize = 200;

        public ProcessBadgeLogic(
            IGeneralBadgeCriteriaResolverLogic<LifeLongBadgeCriteria, Guid> lifeLongCriteriaResolverLogic,
            IProcessTopBadgeQualifiedUserLogic processTopBadgeQualifiedUserLogic,
            WebAppLinkBuilder webAppLinkBuilder,
            IThunderCqrs thunderCqrs,
            BadgeDbContext dbContext)
        {
            _processTopBadgeQualifiedUserLogic = processTopBadgeQualifiedUserLogic;
            _lifeLongCriteriaResolverLogic = lifeLongCriteriaResolverLogic;
            _dbContext = dbContext;
            _webAppLinkBuilder = webAppLinkBuilder;
            _thunderCqrs = thunderCqrs;
        }

        public async Task IssueBadges(Guid badgeId, IAggregateFluent<Guid> userIds, Guid currentUserId, CancellationToken cancellationToken)
        {
            var totalUser = await userIds.CountAsync(cancellationToken);

            if (totalUser == 0)
            {
                return;
            }

            var resultCount = 0;
            var pageIndex = 0;

            while ((pageIndex * _pageSize) + resultCount <= totalUser)
            {
                var pagedUserIds = await userIds
                     .Skip(pageIndex * _pageSize)
                     .Limit(_pageSize)
                     .ToListAsync(cancellationToken);

                pageIndex++;
                resultCount = pagedUserIds.Count;
                await IssueBadges(badgeId, pagedUserIds, currentUserId, cancellationToken);
            }
        }

        public async Task IssueBadges(Guid badgeId, List<Guid> userIds, Guid currentUserId, CancellationToken cancellationToken)
        {
            var existedUserRewards = await _dbContext
                .UserRewardCollection
                .AsQueryableWhere(p => userIds.Contains(p.UserId))
                .ToListAsync(cancellationToken);

            var existedUserRewardsDict = existedUserRewards
                .GroupBy(p => p.UserId)
                .ToDictionary(
                    key => key.Key,
                    value => value.Any(userReward => userReward.BadgeId == badgeId));

            var newUserRewards = userIds
                .Where(userId => !existedUserRewardsDict.GetValueOrDefault(userId))
                .SelectList(userId => new UserReward(userId, badgeId).SetIssuedBy(currentUserId).SetLevel(BadgeLevelEnum.Level1));
            await _dbContext.UserRewardCollection.InsertManyAsync(newUserRewards, cancellationToken: cancellationToken);

            if (badgeId == BadgeIdsConstants._activeContributorBadgeId)
            {
                var lifeLongQualifiedUserIds = await _lifeLongCriteriaResolverLogic.GetQualifiedUserAsync(Clock.Now);
                await _processTopBadgeQualifiedUserLogic.ExecuteAsync(BadgeIdsConstants._activeContributorBadgeId, lifeLongQualifiedUserIds);
            }

            // Send notification for new learner received badges
            await SendAchievementNotificationToLearner(badgeId, newUserRewards, currentUserId, null, cancellationToken);
        }

        private async Task SendAchievementNotificationToLearner(Guid badgeId, List<UserReward> newUserRewards, Guid currentUserId, Guid? communityId = null, CancellationToken cancellationToken = default)
        {
            var badge = await _dbContext.BadgeCollection.FirstOrDefaultAsync(p => p.Id == badgeId, cancellationToken);

            if (badge == null)
            {
                throw new EntityNotFoundException(typeof(BadgeEntity), badgeId);
            }

            // For Collaborative Learners, Digital Learners, Reflective Learners badges.
            if (badgeId == BadgeIdsConstants._collaborativeLearnersBadgeId
                || badgeId == BadgeIdsConstants._reflectiveLearnersBadgeId
                || badgeId == BadgeIdsConstants._digitalLearnersBadgeId)
            {
                await ForNonCommunityBuilderBadges(newUserRewards, currentUserId, badge, true, cancellationToken);
            }

            // For Community Builder badges, when I’m awarded a badge.
            if (badgeId == BadgeIdsConstants._conversationStarterBadgeId
                || badgeId == BadgeIdsConstants._conversationBoosterBadgeId
                || badgeId == BadgeIdsConstants._visualStorytellerBadgeId
                || badgeId == BadgeIdsConstants._linkCuratorBadgeId)
            {
                await ForCommunityBuilderBadges(newUserRewards, currentUserId, badge, communityId, cancellationToken);
            }

            // For Active Contributor, Life-Long Learners badges, when I’m awarded a badge,
            if (badgeId == BadgeIdsConstants._activeContributorBadgeId
                || badgeId == BadgeIdsConstants._lifeLongBadgeId)
            {
                await ForNonCommunityBuilderBadges(newUserRewards, currentUserId, badge, false, cancellationToken);
            }
        }

        private async Task ForCommunityBuilderBadges(List<UserReward> newUserRewards, Guid currentUserId, BadgeEntity badge, Guid? communityId, CancellationToken cancellationToken)
        {
            var community = _dbContext.CommunityCollection.AsQueryableWhere(p => p.Id == communityId).FirstOrDefault();

            var eventNotification = new AchievedBadgesNotifyLearnerEvent(
                    currentUserId,
                    new AchievedBadgesNotifyLearnerPayload
                    {
                        ActionUrl = _webAppLinkBuilder.GetMyAchievementsLearnerLinkForCAMModule(),
                        ParamBadgeText = $@"{badge.Name} badge of Community Builder in community {community?.Name}"
                    },
                    newUserRewards.SelectList(p => p.UserId));

            await _thunderCqrs.SendEvent(eventNotification, cancellationToken);
        }

        private async Task ForNonCommunityBuilderBadges(List<UserReward> newUserRewards, Guid currentUserId, BadgeEntity badge, bool hasBadgeLevel, CancellationToken cancellationToken)
        {
            var userRewardsDic = newUserRewards
                .Where(p => !p.CommunityId.HasValue && p.BadgeId == badge.Id)
                .GroupBy(p => p.UserId)
                .ToDictionary(
                    p => p.Key,
                    p => p.FirstOrDefault());

            var eventNotifications = newUserRewards.Select(p => new AchievedBadgesNotifyLearnerEvent(
                currentUserId,
                new AchievedBadgesNotifyLearnerPayload
                {
                    ActionUrl = _webAppLinkBuilder.GetMyAchievementsLearnerLinkForCAMModule(),
                    ParamBadgeText = hasBadgeLevel ?
                        $@"{userRewardsDic.GetValueOrDefault(p.UserId)?.BadgeLevel} badge of {badge.Name}" :
                        $@"{badge.Name} badge"
                },
                new List<Guid> { p.UserId }));

            await _thunderCqrs.SendEvents(eventNotifications, cancellationToken);
        }
    }
}
