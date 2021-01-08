using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Badge.Application.AggregatedModels;
using Microservice.Badge.Application.BusinessLogic;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Domain.Constants;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using Microservice.Badge.Infrastructure.Helpers;
using MongoDB.Driver;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Badge.Application.HangfireJob
{
    public class SummarizeMonthlyStatisticsJob : BaseHangfireJob, ISummarizeMonthlyStatisticsForBadging
    {
        private readonly IEnsureYearlyStatisticExistLogic _ensureYearlyStatisticExistLogic;
        private readonly IProcessTopBadgeQualifiedUserLogic _processTopBadgeQualifiedUserLogic;
        private readonly IGeneralBadgeCriteriaResolverLogic<ReflectiveLearnersBadgeCriteria, YearlyUserStatistic> _reflectiveBadgeResolver;
        private readonly ICommunityBuilderBadgeCriteriaResolverLogic<LinkCuratorBadgeCriteria> _linkCuratorBadgeResolver;
        private readonly ICommunityBuilderBadgeCriteriaResolverLogic<VisualStorytellerBadgeCriteria> _visualStorytellerBadgeResolver;
        private readonly ICommunityBuilderBadgeCriteriaResolverLogic<ConversationBoosterBadgeCriteria> _conversationBoosterBadgeResolver;
        private readonly ICommunityBuilderBadgeCriteriaResolverLogic<ConversationStarterBadgeCriteria> _conversationStarterBadgeResolver;
        private readonly IGeneralBadgeCriteriaResolverLogic<ActiveContributorsBadgeCriteria, Guid> _activeContributorsBadgeResolver;
        private readonly int _currentYear = Clock.Now.Year;

        public SummarizeMonthlyStatisticsJob(
            BadgeDbContext dbContext,
            IEnsureYearlyStatisticExistLogic ensureYearlyStatisticExistLogic,
            IProcessTopBadgeQualifiedUserLogic processTopBadgeQualifiedUserLogic,
            IGeneralBadgeCriteriaResolverLogic<ReflectiveLearnersBadgeCriteria, YearlyUserStatistic> reflectiveBadgeResolver,
            IGeneralBadgeCriteriaResolverLogic<ActiveContributorsBadgeCriteria, Guid> activeContributorsBadgeResolver,
            ICommunityBuilderBadgeCriteriaResolverLogic<LinkCuratorBadgeCriteria> linkCuratorBadgeResolver,
            ICommunityBuilderBadgeCriteriaResolverLogic<VisualStorytellerBadgeCriteria> visualStorytellerBadgeResolver,
            ICommunityBuilderBadgeCriteriaResolverLogic<ConversationBoosterBadgeCriteria> conversationBoosterBadgeResolver,
            ICommunityBuilderBadgeCriteriaResolverLogic<ConversationStarterBadgeCriteria> conversationStarterBadgeResolver) : base(dbContext)
        {
            _ensureYearlyStatisticExistLogic = ensureYearlyStatisticExistLogic;
            _processTopBadgeQualifiedUserLogic = processTopBadgeQualifiedUserLogic;
            _reflectiveBadgeResolver = reflectiveBadgeResolver;
            _linkCuratorBadgeResolver = linkCuratorBadgeResolver;
            _visualStorytellerBadgeResolver = visualStorytellerBadgeResolver;
            _conversationBoosterBadgeResolver = conversationBoosterBadgeResolver;
            _conversationStarterBadgeResolver = conversationStarterBadgeResolver;
            _activeContributorsBadgeResolver = activeContributorsBadgeResolver;
        }

        protected override async Task InternalHandleAsync(CancellationToken cancellationToken = default)
        {
            var allWriteModelTaskList = new List<Task<ReplaceOneModel<YearlyUserStatistic>>>();
            await HangfireHelper.PerformBatchJob(
                GetLastUserStatisticsInPreviousMonth(),
                monthlyUserStatistics => allWriteModelTaskList.AddRange(monthlyUserStatistics.Select(AddMonthlyUserStatistic)),
                cancellationToken);

            var allWriteModels = await Task.WhenAll(allWriteModelTaskList);
            await BadgeDbContext.YearlyUserStatisticCollection.BulkWriteAsync(allWriteModels, cancellationToken: cancellationToken);

            var qualifiedLinkCuratorUsers = await _linkCuratorBadgeResolver.GetQualifiedUserAsync(Clock.Now);
            await UpdateCommunityBuilderTopUser(qualifiedLinkCuratorUsers, BadgeIdsConstants._linkCuratorBadgeId);

            var qualifiedStorytellerUsers = await _visualStorytellerBadgeResolver.GetQualifiedUserAsync(Clock.Now);
            await UpdateCommunityBuilderTopUser(qualifiedStorytellerUsers, BadgeIdsConstants._visualStorytellerBadgeId);

            var qualifiedConversationBoosterUsers = await _conversationBoosterBadgeResolver.GetQualifiedUserAsync(Clock.Now);
            await UpdateCommunityBuilderTopUser(qualifiedConversationBoosterUsers, BadgeIdsConstants._conversationBoosterBadgeId);

            var qualifiedConversationStarterUsers = await _conversationStarterBadgeResolver.GetQualifiedUserAsync(Clock.Now);
            await UpdateCommunityBuilderTopUser(qualifiedConversationStarterUsers, BadgeIdsConstants._conversationStarterBadgeId);

            var qualifiedReflectiveUsers = await _reflectiveBadgeResolver.GetQualifiedUserAsync(Clock.Now);
            await _processTopBadgeQualifiedUserLogic.ExecuteAsync(BadgeIdsConstants._reflectiveLearnersBadgeId, qualifiedReflectiveUsers);

            var qualifiedActiveContributors = await _activeContributorsBadgeResolver.GetQualifiedUserAsync(Clock.Now);
            await _processTopBadgeQualifiedUserLogic.ExecuteAsync(BadgeIdsConstants._activeContributorBadgeId, qualifiedActiveContributors);
        }

        private async Task<ReplaceOneModel<YearlyUserStatistic>> AddMonthlyUserStatistic(
            MonthlyStatisticEnumerationModel monthlyUserStatistic)
        {
            var yearlyStatistic = await _ensureYearlyStatisticExistLogic.ByYear(monthlyUserStatistic.UserId, _currentYear);
            yearlyStatistic.SetMonthlyStatistic(monthlyUserStatistic.DailyStatistics);

            return new ReplaceOneModel<YearlyUserStatistic>(
                Builders<YearlyUserStatistic>.Filter.Eq(p => p.Id, yearlyStatistic.Id),
                yearlyStatistic);
        }

        private IAggregateFluent<MonthlyStatisticEnumerationModel> GetLastUserStatisticsInPreviousMonth()
        {
            // Get UTC date in current time zone.
            var now = Clock.Now.EndOfDateInSystemTimeZone().ToUtcFromSystemTimeZone();
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            return BadgeDbContext.YearlyUserStatisticCollection
                .Aggregate(AggregateOptions)
                .Unwind<YearlyUserStatistic, MonthlyStatisticEnumerationModel>(x => x.DailyStatistics)
                .SortByDescending(x => x.Year)
                .ThenByDescending(x => x.DailyStatistics.ExecutedDate)
                .Match(Builders<MonthlyStatisticEnumerationModel>.Filter.Lt(x => x.DailyStatistics.ExecutedDate, firstDayOfMonth))
                .Group(x => x.UserId, g => new
                {
                    g.Key,
                    DailyStatistic = g.Select(x => x.DailyStatistics).First()
                })
                .Project(x => new MonthlyStatisticEnumerationModel
                {
                    UserId = x.Key,
                    DailyStatistics = x.DailyStatistic
                });
        }

        /// <summary>
        /// Update community builder top qualified user.
        /// </summary>
        /// <param name="communityBuilderAggregates">List CommunityStatisticAggregateModel of each community.</param>
        /// <returns>Task.</returns>
        private async Task UpdateCommunityBuilderTopUser(List<IAggregateFluent<CommunityStatisticAggregateModel>> communityBuilderAggregates, Guid badgeId)
        {
            // TODO: Refactor for higher performance
            var userDictionary = new Dictionary<Guid, UserEntity>();
            foreach (var aggregate in communityBuilderAggregates)
            {
                var currentStatistic = await aggregate.ToListAsync();
                if (currentStatistic.Count == 0)
                {
                    continue;
                }

                var communityId = currentStatistic.First().CommunityId;
                var userStatistics = currentStatistic.SelectMany(x => x.UserStatistics).ToList();
                var userIds = userStatistics.Where(p => !userDictionary.ContainsKey(p.UserId)).SelectListDistinct(x => x.UserId);
                var users = await BadgeDbContext
                    .UserCollection
                    .AsQueryableWhere(p => userIds.Contains(p.Id))
                    .ToListAsync();
                users.ForEach(x => userDictionary.TryAdd(x.Id, x));

                var topQualifiedUserBadges = userStatistics.SelectList(p =>
                {
                    var currentUser = userDictionary.GetValueOrDefault(p.UserId);
                    var topBadgeQualifiedUser = new TopBadgeQualifiedUser(badgeId, p.UserId)
                        .SetCommunityId(communityId)
                        .UpdateUserInfo(currentUser?.FirstName, currentUser?.LastName, currentUser?.Email);
                    return badgeId == BadgeIdsConstants._conversationBoosterBadgeId
                        ? topBadgeQualifiedUser.SetNumOfCreatedForum(p.Count)
                        : topBadgeQualifiedUser.SetNumOfQualifiedPost(p.Count);
                });

                await BadgeDbContext
                    .TopBadgeQualifiedUserCollection
                    .InsertManyAsync(topQualifiedUserBadges);
            }
        }
    }
}
