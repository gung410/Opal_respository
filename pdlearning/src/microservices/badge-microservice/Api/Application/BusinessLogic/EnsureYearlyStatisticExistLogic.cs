using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Badge.Application.BusinessLogic
{
    public class EnsureYearlyStatisticExistLogic : IEnsureYearlyStatisticExistLogic
    {
        private readonly BadgeDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureYearlyStatisticExistLogic"/> class.
        /// This service is used to ensure have Yearly Statistic record for user. It can be used in Service, Consumer, Command and maybe Query.
        /// </summary>
        /// <param name="dbContext">BadgeDbContext.</param>
        public EnsureYearlyStatisticExistLogic(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<YearlyUserStatistic> ByYear(Guid userId, int year)
        {
            var yearlyStatistic = await GetYearlyStatisticByYear(userId, year);

            if (yearlyStatistic != null)
            {
                return yearlyStatistic;
            }

            await _dbContext.YearlyUserStatisticCollection.InsertOneAsync(new YearlyUserStatistic(userId, year));
            return await GetYearlyStatisticByYear(userId, year);
        }

        public async Task<CommunityYearlyUserStatistic> ByYearAndCommunity(Guid userId, int year, Guid communityId)
        {
            var yearlyStatistic = await GetCommunityYearlyStatisticByYear(userId, year, communityId);

            if (yearlyStatistic != null)
            {
                return yearlyStatistic;
            }

            await _dbContext.CommunityYearlyUserStatisticCollection.InsertOneAsync(new CommunityYearlyUserStatistic(userId, communityId, year));
            return await GetCommunityYearlyStatisticByYear(userId, year, communityId);
        }

        public async Task<List<CommunityYearlyUserStatistic>> ByYearAndCommunity(List<Guid> userIds, int year, Guid communityId, CancellationToken cancellationToken = default)
        {
            var communityYearlyStatistics = await GetCommunityYearlyStatisticsByYear(userIds, year, communityId, cancellationToken);
            var newUserIds = userIds.Except(communityYearlyStatistics.SelectList(s => s.UserId)).ToList();

            if (newUserIds.Any())
            {
                var newStats = newUserIds.Select(p => new CommunityYearlyUserStatistic(p, communityId, year)).ToList();
                await _dbContext
                    .CommunityYearlyUserStatisticCollection
                    .InsertManyAsync(newStats, null, cancellationToken);
                communityYearlyStatistics = await GetCommunityYearlyStatisticsByYear(userIds, year, communityId, cancellationToken);
            }

            return communityYearlyStatistics;
        }

        private Task<YearlyUserStatistic> GetYearlyStatisticByYear(Guid userId, int year, CancellationToken cancellationToken = default)
        {
            return _dbContext
                .YearlyUserStatisticCollection
                .AsQueryable()
                .Where(x => x.Year == year && x.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        private Task<List<CommunityYearlyUserStatistic>> GetCommunityYearlyStatisticsByYear(List<Guid> userIds, int year, Guid communityId, CancellationToken cancellationToken = default)
        {
            return _dbContext
                .CommunityYearlyUserStatisticCollection
                .AsQueryableWhere(p => p.Year == year && userIds.Contains(p.UserId) && p.CommunityId == communityId)
                .ToListAsync(cancellationToken);
        }

        private Task<CommunityYearlyUserStatistic> GetCommunityYearlyStatisticByYear(Guid userId, int year, Guid communityId, CancellationToken cancellationToken = default)
        {
            return _dbContext
                .CommunityYearlyUserStatisticCollection
                .AsQueryable()
                .Where(x => x.Year == year && x.UserId == userId && x.CommunityId == communityId)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
