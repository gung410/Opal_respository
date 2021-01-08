using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Infrastructure;
using MongoDB.Driver;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Badge.Application.BusinessLogic
{
    public class ProcessTopBadgeQualifiedUserLogic : ApplicationService, IProcessTopBadgeQualifiedUserLogic
    {
        private readonly BadgeDbContext _dbContext;

        public ProcessTopBadgeQualifiedUserLogic(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ExecuteAsync(Guid badgeId, IAggregateFluent<Guid> qualifiedUserIds)
        {
            if (qualifiedUserIds == null)
            {
                return;
            }

            int pageIndex = 0;
            int pageSize = 100;
            while (true)
            {
                var pagedUserIds = await qualifiedUserIds.Skip(pageIndex * pageSize).Limit(pageSize).ToListAsync();
                if (pagedUserIds.Count == 0)
                {
                    return;
                }

                var topBadgeQualifiedUsers = pagedUserIds.SelectList(p => new TopBadgeQualifiedUser(badgeId, p));
                await _dbContext.TopBadgeQualifiedUserCollection.InsertManyAsync(topBadgeQualifiedUsers);

                pageIndex++;
            }
        }

        public async Task ExecuteAsync(Guid badgeId, IAggregateFluent<YearlyUserStatistic> qualifiedUserStatistic)
        {
            if (qualifiedUserStatistic == null)
            {
                return;
            }

            int pageIndex = 0;
            int pageSize = 100;
            while (true)
            {
                var qualifiedStatistic = await qualifiedUserStatistic.Skip(pageIndex * pageSize).Limit(pageSize).ToListAsync();
                if (qualifiedStatistic.Count == 0)
                {
                    return;
                }

                var userIds = qualifiedStatistic.SelectList(x => x.UserId);
                var users = await _dbContext
                    .UserCollection
                    .Aggregate()
                    .Match(x => userIds.Contains(x.Id))
                    .ToListAsync();
                var userDict = users.ToDictionary(x => x.Id);

                var newTopBadgeUsers = qualifiedStatistic.SelectList(x =>
                new TopBadgeQualifiedUser(badgeId, x.UserId)
                .UpdateUserInfo(userDict.GetValueOrDefault(x.UserId)?.FirstName, userDict.GetValueOrDefault(x.UserId)?.LastName, userDict.GetValueOrDefault(x.UserId)?.Email)
                .UpdateGeneralStatistic(x.LatestMonthlyStatistics));

                await _dbContext.TopBadgeQualifiedUserCollection.InsertManyAsync(newTopBadgeUsers);

                pageIndex++;
            }
        }
    }
}
