using System;
using System.Linq;
using Microservice.Badge.Application.AggregatedModels;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Domain.Constants;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using Thunder.Platform.Core.Timing;

namespace Microservice.Badge.Application.BusinessLogic
{
    public class ActiveContributorsBadgeCriteriaResolverLogic : BaseGeneralBadgeCriteriaResolverLogic<ActiveContributorsBadgeCriteria, Guid>
    {
        private readonly BadgeDbContext _dbContext;

        public ActiveContributorsBadgeCriteriaResolverLogic(
            BadgeDbContext dbContext,
            IGetBadgeCriteriaLogic<ActiveContributorsBadgeCriteria> badgeCriteriaStorage) : base(dbContext, badgeCriteriaStorage)
        {
            _dbContext = dbContext;
        }

        public override IAggregateFluent<Guid> ResolveCriteriaAsync(ActiveContributorsBadgeCriteria badgeCriteria, DateTime statisticDatetime)
        {
            // Current month is end of ExecuteMonth;
            // Ex: ExecuteMonth = 1 => Current month = 2, ExecuteMonth = 12 => Current month = 1 (next year)
            var prevMonthDateTime = Clock.Now.AddMonths(-1);

            if (prevMonthDateTime.Month == badgeCriteria.ExecuteMonth)
            {
                return _dbContext
                    .UserRewardCollection
                    .Aggregate()
                    .Group(g => g.UserId, g => new UserRewardAggregatedModel
                    {
                        Id = g.Key,
                        UserRewards = g.Select(x => new UserBadgeModel
                        {
                            BadgeId = x.BadgeId,
                            BadgeLevel = x.BadgeLevel
                        })
                    })
                    .Match(Builders<UserRewardAggregatedModel>.Filter.ElemMatch(
                        p => p.UserRewards,
                        r => (badgeCriteria.LevelOfCollaborativeLearnersBadge == null ||
                                (r.BadgeId == BadgeIdsConstants._collaborativeLearnersBadgeId && r.BadgeLevel.Level == badgeCriteria.LevelOfCollaborativeLearnersBadge)) &&
                             (badgeCriteria.LevelOfDigitalLearnersBadge == null ||
                                (r.BadgeId == BadgeIdsConstants._digitalLearnersBadgeId && r.BadgeLevel.Level == badgeCriteria.LevelOfDigitalLearnersBadge)) &&
                             (badgeCriteria.LevelOfReflectiveLearnersBadge == null ||
                                (r.BadgeId == BadgeIdsConstants._reflectiveLearnersBadgeId && r.BadgeLevel.Level == badgeCriteria.LevelOfReflectiveLearnersBadge)) &&
                             (badgeCriteria.CommunityBadgesIds == null || !badgeCriteria.CommunityBadgesIds.Any() || badgeCriteria.CommunityBadgesIds.Contains(r.BadgeId))))
                    .Lookup(
                        _dbContext.YearlyUserStatisticCollection,
                        x => x.Id,
                        y => y.UserId,
                        (UserRewardYearlyUserStatisticAggregatedModel r) => r.YearlyUserStatistic)
                    .Unwind(p => p.YearlyUserStatistic)
                    .Match(Builders<BsonDocument>.Filter.Eq($"{nameof(UserRewardYearlyUserStatisticAggregatedModel.YearlyUserStatistic)}.{nameof(YearlyUserStatistic.Year)}", prevMonthDateTime.Year))
                    .Sort(Builders<BsonDocument>.Sort.Descending($"{nameof(UserRewardYearlyUserStatisticAggregatedModel.YearlyUserStatistic)}.{nameof(YearlyUserStatistic.LatestMonthlyStatistics)}.{nameof(GeneralStatistic.ActiveContributorActivityTotal)}"))
                    .Select(p => (Guid)p["_id"]);
            }

            return null;
        }
    }
}
