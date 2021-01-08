using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Badge.Application.BusinessLogic;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Domain.Constants;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using Microservice.Badge.Infrastructure.Helpers;
using MongoDB.Driver;
using Thunder.Platform.Core.Timing;

namespace Microservice.Badge.Application.HangfireJob
{
    // todo: separate different jobs
    public class SummarizeDailyStatisticsJob : BaseHangfireJob, ISummarizeDailyStatisticsForBadging
    {
        private readonly IProcessBadgeLogic _processBadgeLogic;
        private readonly IEnsureYearlyStatisticExistLogic _ensureYearlyStatisticExistLogic;
        private readonly IGeneralBadgeCriteriaResolverLogic<CollaborativeLearnersBadgeCriteria, YearlyUserStatistic> _collaborativeLearnersResolver;

        public SummarizeDailyStatisticsJob(
            BadgeDbContext dbContext,
            IProcessBadgeLogic processBadgeLogic,
            IGeneralBadgeCriteriaResolverLogic<CollaborativeLearnersBadgeCriteria, YearlyUserStatistic> collaborativeLearnersResolver,
            IEnsureYearlyStatisticExistLogic ensureYearlyStatisticExistLogic) : base(dbContext)
        {
            _processBadgeLogic = processBadgeLogic;
            _ensureYearlyStatisticExistLogic = ensureYearlyStatisticExistLogic;
            _collaborativeLearnersResolver = collaborativeLearnersResolver;
        }

        protected override async Task InternalHandleAsync(CancellationToken cancellationToken = default)
        {
            await UpdateGeneralStatistic(cancellationToken);
            await UpdateCommunityStatistic(cancellationToken);

            // TODO: Consider moving to another job.
            var qualifiedUsers = await _collaborativeLearnersResolver.GetQualifiedUserAsync(Clock.Now);
            var userIdsWinBadge = qualifiedUsers.Select(p => p.UserId);
            await _processBadgeLogic.IssueBadges(BadgeIdsConstants._collaborativeLearnersBadgeId, userIdsWinBadge, Guid.Empty, cancellationToken);
        }

        private async Task UpdateGeneralStatistic(CancellationToken cancellationToken = default)
        {
            var allWriteModelTaskList = new List<Task<ReplaceOneModel<YearlyUserStatistic>>>();
            await HangfireHelper.PerformBatchJob(
                GetUserActivities(),
                userStatistics => allWriteModelTaskList.AddRange(userStatistics.Select(AddDailyUserStatistic)),
                cancellationToken);

            var allWriteModels = await Task.WhenAll(allWriteModelTaskList);
            await BadgeDbContext.YearlyUserStatisticCollection.BulkWriteAsync(allWriteModels, cancellationToken: cancellationToken);
        }

        private IAggregateFluent<UserStatisticEnumerationModel> GetUserActivities()
        {
            return BadgeDbContext
                .ActivityCollection
                .Aggregate(AggregateOptions)
                .Group(
                    x => new { x.UserId, x.Type },
                    g => new { g.Key, Count = g.Sum(a => 1) })
                .Group(
                    x => new { x.Key.UserId },
                    g =>
                        new
                        {
                            g.Key,
                            Activities = g.Select(x => new ActivityEnumerationModel
                            {
                                Type = x.Key.Type,
                                Count = x.Count
                            })
                        })
                .Project(x => new UserStatisticEnumerationModel
                {
                    UserId = x.Key.UserId,
                    Activities = x.Activities
                });
        }

        private async Task<ReplaceOneModel<YearlyUserStatistic>> AddDailyUserStatistic(UserStatisticEnumerationModel userStatistic)
        {
            var dictUserActivities = userStatistic.Activities.ToDictionary(p => p.Type, p => p.Count);
            GeneralStatistic dailyStatistic = new(dictUserActivities);
            var yearlyStatistic = await _ensureYearlyStatisticExistLogic.ByYear(userStatistic.UserId, dailyStatistic.ExecutedDate.Year);
            yearlyStatistic.SetDailyStatistic(dailyStatistic);

            return new ReplaceOneModel<YearlyUserStatistic>(
                Builders<YearlyUserStatistic>.Filter.Eq(p => p.Id, yearlyStatistic.Id),
                yearlyStatistic);
        }

        private async Task UpdateCommunityStatistic(CancellationToken cancellationToken = default)
        {
            var allWriteModelTaskList = new List<Task<ReplaceOneModel<CommunityYearlyUserStatistic>>>();
            await HangfireHelper.PerformBatchJob(
                GetUserConversationStarterActivities(),
                items =>
                {
                    var communityWriteModelTasks = items.Select(userCommunityStatistic =>
                    {
                        var group = userCommunityStatistic.CommunityActivities.GroupBy(p => p.Type);
                        var createdForum = group
                            .Where(p => p.Key == ActivityType.CreateForum)
                            .SelectMany(p => p)
                            .ToList();
                        var post = group
                            .Where(p => p.Key == ActivityType.PostCommunity)
                            .SelectMany(p => p)
                            .ToList();
                        var interaction = group.Where(a =>
                                a.Key == ActivityType.CommentOthersPost
                                || a.Key == ActivityType.CommentSelfPost
                                || a.Key == ActivityType.LikePost)
                            .SelectMany(p => p)
                            .ToList();

                        CommunityStatistic communityStatistic = new()
                        {
                            NumOfCreatedForum = createdForum.Count,
                            NumOfCreatedPost = post.Count,
                            NumOfInteractions = interaction.Count
                        };
                        return AddDailyCommunityStatistic(
                            userCommunityStatistic.UserId,
                            userCommunityStatistic.CommunityId,
                            communityStatistic);
                    });

                    allWriteModelTaskList.AddRange(communityWriteModelTasks);
                },
                cancellationToken);

            var allWriteModels = await Task.WhenAll(allWriteModelTaskList);
            await BadgeDbContext.CommunityYearlyUserStatisticCollection.BulkWriteAsync(allWriteModels, cancellationToken: cancellationToken);
        }

        private IAggregateFluent<UserCommunityStatisticEnumerationModel> GetUserConversationStarterActivities()
        {
            var userStatistics = BadgeDbContext
                .ActivityCollection
                .Aggregate(AggregateOptions)
                .Match(a => a.Type == ActivityType.PostCommunity

                            || a.Type == ActivityType.CreateForum

                            || a.Type == ActivityType.CommentOthersPost
                            || a.Type == ActivityType.CommentSelfPost
                            || a.Type == ActivityType.LikePost)
                .Sort(Builders<UserActivity>.Sort.Ascending(p => p.ActivityDate))
                .Group(
                    x => new { x.UserId, x.CommunityInfo.CommunityId, x.Type, x.SourceId, x.CommunityInfo.PostId },
                    g => new { g.Key, Count = g.Sum(a => 1) })
                .Group(
                    x => new { x.Key.UserId, x.Key.CommunityId },
                    g =>
                        new
                        {
                            g.Key,
                            Activities = g.Select(x => new CommunityActivityEnumerationModel
                            {
                                Type = x.Key.Type,
                                PostId = x.Key.PostId,
                                SourceId = x.Key.SourceId,
                            })
                        })
                .Project(x => new UserCommunityStatisticEnumerationModel
                {
                    UserId = x.Key.UserId,
                    CommunityId = x.Key.CommunityId,
                    CommunityActivities = x.Activities
                });

            return userStatistics;
        }

        private async Task<ReplaceOneModel<CommunityYearlyUserStatistic>> AddDailyCommunityStatistic(
            Guid userId,
            Guid communityId,
            CommunityStatistic dailyCommunityStatistic)
        {
            var year = dailyCommunityStatistic.ExecutedDate.Year;
            var yearlyStatistic = await _ensureYearlyStatisticExistLogic.ByYearAndCommunity(userId, year, communityId);
            yearlyStatistic.SetDailyStatistic(dailyCommunityStatistic);
            return new ReplaceOneModel<CommunityYearlyUserStatistic>(
                Builders<CommunityYearlyUserStatistic>.Filter.Eq(p => p.Id, yearlyStatistic.Id), yearlyStatistic);
        }
    }
}
