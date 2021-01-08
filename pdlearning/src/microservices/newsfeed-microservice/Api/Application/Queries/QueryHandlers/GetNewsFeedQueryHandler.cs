using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.NewsFeed.Application.Models;
using Microservice.NewsFeed.Domain.Entities;
using Microservice.NewsFeed.Infrastructure;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Thunder.Platform.Cqrs;

namespace Microservice.NewsFeed.Application.Queries.QueryHandlers
{
    public class GetNewsFeedQueryHandler : BaseThunderQueryHandler<GetNewsFeedQuery, FeedResultModel>
    {
        private readonly NewsFeedDbContext _dbContext;

        public GetNewsFeedQueryHandler(NewsFeedDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task<FeedResultModel> HandleAsync(GetNewsFeedQuery query, CancellationToken cancellationToken)
        {
            var newFeedQuery = _dbContext.NewsFeedCollection
                .AsQueryable()
                .Where(p => p.UserId == query.UserId);

            var totalCount = await newFeedQuery.CountAsync(cancellationToken);

            var newsFeeds = await newFeedQuery
                .OrderByDescending(p => p.CreatedDate)
                .Skip(query.SkipCount)
                .Take(query.MaxResultCount)
                .ToListAsync(cancellationToken);

            object[] newFeeds = newsFeeds.Select(p =>
            {
                switch (p.Type)
                {
                    case nameof(PdpmSuggestCourseFeed):
                        return new CourseFeedModel((PdpmSuggestCourseFeed)p);
                    case nameof(UserPostFeed):
                        return new UserPostFeedModel((UserPostFeed)p);
                    case nameof(CommunityPostFeed):
                        return (object)new CommunityFeedModel((CommunityPostFeed)p);
                }

                return null;
            }).ToArray();

            return new FeedResultModel(newFeeds, totalCount);
        }
    }
}
