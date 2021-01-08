using System;
using Microservice.NewsFeed.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.NewsFeed.Application.Queries
{
    public class GetNewsFeedQuery : BaseThunderQuery<FeedResultModel>
    {
        public Guid UserId { get; set; }

        public int SkipCount { get; set; }

        public int MaxResultCount { get; set; }
    }
}
