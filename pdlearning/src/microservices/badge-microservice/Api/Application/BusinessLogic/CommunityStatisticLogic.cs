using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Badge.Application.Consumers.Dtos;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using MongoDB.Driver;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Badge.Application.BusinessLogic
{
    // todo: move it to hangfire job
    public class CommunityStatisticLogic : ICommunityStatisticLogic
    {
        private readonly BadgeDbContext _dbContext;
        private readonly IEnsureYearlyStatisticExistLogic _ensureYearlyStatisticExistLogic;

        private readonly int _pageSize;

        public CommunityStatisticLogic(
            BadgeDbContext dbContext,
            IEnsureYearlyStatisticExistLogic ensureYearlyStatisticExistLogic,
            int pageSize = 200)
        {
            _dbContext = dbContext;
            _pageSize = pageSize;
            _ensureYearlyStatisticExistLogic = ensureYearlyStatisticExistLogic;
        }

        public async Task ProcessAsync(CancellationToken cancellationToken = default)
        {
            var index = 0;
            while (true)
            {
                var skip = index * _pageSize;
                var userReports = await GetUserReportsAsync(skip, _pageSize, cancellationToken);
                if (userReports.Count <= 0)
                {
                    return;
                }

                await UpdateCommunityYearlyUserStatistics(userReports, cancellationToken);
            }
        }

        private async Task UpdateCommunityYearlyUserStatistics(List<UserCommunityReportDto> reports, CancellationToken cancellationToken)
        {
            var year = Clock.Now.Year;
            var userCommunityReportGroup = reports.GroupBy(p => p.CommunityId);
            foreach (var item in userCommunityReportGroup)
            {
                var userIds = item.SelectList(p => p.UserId);
                var communityYearlyUserStatistics = await _ensureYearlyStatisticExistLogic.ByYearAndCommunity(userIds, year, item.Key, cancellationToken);

                // todo: refactor to update many
                await UpdateDailyStats(communityYearlyUserStatistics, reports, cancellationToken);
            }
        }

        private async Task<List<CommunityYearlyUserStatistic>> UpdateDailyStats(
            List<CommunityYearlyUserStatistic> yearlyStats,
            List<UserCommunityReportDto> reports,
            CancellationToken cancellationToken)
        {
            foreach (var yearlyStat in yearlyStats)
            {
                var report = reports.First(r => r.UserId == yearlyStat.UserId);
                yearlyStat.SetDailyStatistic(report.ToCommunityStatistic());
                await _dbContext.CommunityYearlyUserStatisticCollection
                        .ReplaceOneAsync(x => x.Id == yearlyStat.Id, yearlyStat, cancellationToken: cancellationToken);
            }

            return yearlyStats;
        }

        private async Task<List<UserCommunityReportDto>> GetUserReportsAsync(
            int skip,
            int limit,
            CancellationToken cancellationToken)
        {
            var userPostResponses = await GetPostResponsesReports(skip, limit, cancellationToken);
            var userIds = userPostResponses.Select(r => r.UserId).ToList();
            var userPosts = await GetUserPostsReports(userIds, cancellationToken);
            var userReports = GetReports(userPostResponses, userPosts);

            return userReports;
        }

        private Task<List<CommunityUserPostResponsesReport>> GetPostResponsesReports(int skip, int limit, CancellationToken cancellationToken)
        {
            return _dbContext
                .ActivityCollection
                .Aggregate()
                .Match(a => a.Type == ActivityType.CommentOthersPost)
                .Group(
                a => new { a.UserId, a.CommunityInfo.CommunityId, a.CommunityInfo.PostId },
                g => new
                {
                    g.Key,
                    NumOfComments = g.Sum(a => 1)
                })
                .Group(
                a => new { a.Key.UserId, a.Key.CommunityId },
                g => new
                {
                    g.Key,
                    Posts = g.Select(r => new
                    {
                        r.Key.PostId,
                        r.NumOfComments
                    })
                })
                .Group(
                a => new { a.Key.UserId },
                g => new CommunityUserPostResponsesReport
                {
                    UserId = g.First().Key.UserId,
                    Communities = g.Select(r => new CommunityPost
                    {
                        CommunityId = r.Key.CommunityId,
                        Posts = r.Posts.Select(p => new CommunityPostResponses
                        {
                            PostId = p.PostId.ToString(),
                            NumOfResponses = p.NumOfComments
                        })
                    })
                })
                .Skip(skip)
                .Limit(limit)
                .ToListAsync(cancellationToken);
        }

        private Task<List<CommunityUserPostResponsesReport>> GetUserPostsReports(IList<Guid> userIds, CancellationToken cancellationToken)
        {
            return _dbContext
                .ActivityCollection
                .Aggregate()
                .Match(a => a.Type == ActivityType.PostCommunity && userIds.Contains(a.UserId))
                .Group(
                a => new { a.UserId, a.CommunityInfo.CommunityId },
                g => new
                {
                    g.Key,
                    NumOfPosts = g.Sum(r => 1),
                    Posts = g.Select(r => new
                    {
                        r.CommunityInfo.PostId
                    })
                })
                .Group(
                a => new { a.Key.UserId },
                g => new CommunityUserPostResponsesReport
                {
                    UserId = g.First().Key.UserId,
                    Communities = g.Select(r => new CommunityPost
                    {
                        CommunityId = r.Key.CommunityId,
                        NumOfPosts = r.NumOfPosts,
                        Posts = r.Posts.Select(p => new CommunityPostResponses
                        {
                            PostId = p.PostId.ToString()
                        })
                    })
                })
                .ToListAsync(cancellationToken);
        }

        private List<UserCommunityReportDto> GetReports(
            List<CommunityUserPostResponsesReport> usersPostResponses,
            List<CommunityUserPostResponsesReport> userPosts)
        {
            var userReports = new List<UserCommunityReportDto>();
            foreach (var userPost in userPosts)
            {
                var userCommunityPostsResponses = usersPostResponses.Where(ups => ups.UserId == userPost.UserId).SelectMany(ups => ups.Communities);
                foreach (var communityPosts in userPost.Communities)
                {
                    var specificUserCommunityPostResponses = userCommunityPostsResponses
                        .FirstOrDefault(r => r.CommunityId == communityPosts.CommunityId)?
                        .Posts;
                    UserCommunityReportDto userReport = new()
                    {
                        UserId = userPost.UserId,
                        CommunityId = communityPosts.CommunityId,
                        Posts = communityPosts.Posts.Select(p =>
                            new CommunityPostResponses
                            {
                                PostId = p.PostId,
                                NumOfResponses = specificUserCommunityPostResponses?.FirstOrDefault(r => r.PostId == p.PostId)?.NumOfResponses ?? 0
                            })
                        .ToList()
                    };

                    userReports.Add(userReport);
                }
            }

            return userReports;
        }
    }
}
