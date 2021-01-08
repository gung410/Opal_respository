using System;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.NewsFeed.Application.Consumers.Messages;
using Microservice.NewsFeed.Domain.Entities;
using Microservice.NewsFeed.Infrastructure;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Thunder.Platform.Core.Exceptions;
using Community = Microservice.NewsFeed.Domain.Entities.Community;
using User = Microservice.NewsFeed.Domain.Entities.User;

namespace Microservice.NewsFeed.Application.Consumers
{
    [OpalConsumer("csl.events.post.created")]
    public class UserPostCreatedConsumer : OpalMessageConsumer<UserPostMessage>
    {
        private readonly ILogger<UserPostCreatedConsumer> _logger;
        private readonly NewsFeedDbContext _dbContext;

        public UserPostCreatedConsumer(
            ILogger<UserPostCreatedConsumer> logger,
            NewsFeedDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(UserPostMessage message)
        {
            switch (message.SourceType)
            {
                // Case 1: Posts are public and user self-posted or forwarded to the user wall
                case Post.FromUserWall:
                    if (message.IsPostForward())
                    {
                        await InsertIntoNewsFeedForForwardedUser(message);
                    }
                    else if (message.IsUserSelfPosted() && message.IsPostPublic())
                    {
                        await InsertIntoNewsFeedForFollowers(message);
                    }

                    break;

                // Case 2: Posts by community members
                // (posts forwarded to the community have no display on news feed)
                case Post.FromCommunity when !message.IsPostForward():
                    await InsertIntoNewsFeedForMembersCommunity(message);
                    break;
            }

            await InsertIntoPost(message);
        }

        private async Task InsertIntoNewsFeedForFollowers(UserPostMessage message)
        {
            // Posts by user self-posted on wall => Post owner Id
            // otherwise the identifier user is forwarded.
            var userId = message.Source.UserId;
            if (!userId.HasValue)
            {
                return;
            }

            var existingUser = await _dbContext
                .SyncedUserCollection
                .AsQueryable()
                .Where(User.FilterByExtIdExpr(userId.Value))
                .Select(p => new
                {
                    p.ExtId,
                    p.Followers
                })
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                _logger.LogError("UserPostCreated: User not found.");
                return;
            }

            if (existingUser.Followers.Any())
            {
                var userPostsFeed = existingUser.Followers
                    .Select(followerId =>
                    {
                        var userPostFeed = MappingUserPostFeedFrom(message);
                        userPostFeed.AddUserId(followerId);

                        return userPostFeed;
                    })
                    .ToList();

                await _dbContext.NewsFeedCollection.InsertManyAsync(userPostsFeed);
            }
        }

        private async Task InsertIntoNewsFeedForMembersCommunity(UserPostMessage message)
        {
            var validGuid = Guid.TryParse(message.Source.Id.ToString(), out Guid communityId);
            if (!validGuid)
            {
                _logger.LogError("UserPostCreated: Cannot parse community Id.");
                return;
            }

            var community = await _dbContext
                .SyncedCommunityCollection
                .AsQueryable()
                .Where(Community.FilterByIdExpr(communityId))
                .Select(p => new
                {
                    p.CommunityId,
                    p.Membership
                })
                .FirstOrDefaultAsync();

            if (community == null)
            {
                _logger.LogError("UserPostCreated: Community not found.");
                return;
            }

            var communityPostsFeed = community.Membership
                .Select(userId =>
                {
                    var communityPostFeed = MappingCommunityPostFeedFrom(message);
                    communityPostFeed.AddUserId(userId);

                    return communityPostFeed;
                })
                .ToList();

            await _dbContext.NewsFeedCollection.InsertManyAsync(communityPostsFeed);
        }

        /// <summary>
        /// Only posts that have been forwarded to the user wall are supported.
        /// </summary>
        /// <param name="message">Message listen from the CSL module.</param>
        /// <returns>No results are returned.</returns>
        private async Task InsertIntoNewsFeedForForwardedUser(UserPostMessage message)
        {
            var userPostFeed = MappingUserPostFeedFrom(message);

            var contentForward = message.ContentForward;

            // The post has been forwarded from Community.
            if (message.IsPostForwardedFromCommunity())
            {
                var postContentForward = new UserPostForward
                {
                    CreatedDate = contentForward.CreatedAt,
                    ChangedDate = contentForward.UpdatedAt,
                    PostContent = contentForward.PostContent,
                    PostId = contentForward.PostId,
                    PostType = contentForward.PostType,
                    PostedBy = contentForward.CreatedBy,
                    Url = contentForward.Source.Url,
                    CommunityId = Guid.Parse(contentForward.Source.Id.ToString()),
                    CommunityName = contentForward.Source.CommunityName,
                    CommunityThumbnailUrl = contentForward.Source.CommunityThumbnailUrl,
                    Description = contentForward.Source.CommunityDescription,
                    Type = nameof(CommunityPostFeed)
                };

                userPostFeed.AddPostForward(postContentForward);
            }

            // The post has been forwarded from user wall.
            else if (message.IsPostForwardedFromUserPost())
            {
                var postContentForward = new UserPostForward
                {
                    CreatedDate = contentForward.CreatedAt,
                    ChangedDate = contentForward.UpdatedAt,
                    PostContent = contentForward.PostContent,
                    PostId = contentForward.PostId,
                    PostType = contentForward.PostType,
                    PostedBy = contentForward.CreatedBy,
                    Url = contentForward.Source.Url,
                    Type = nameof(UserPostFeed)
                };

                userPostFeed.AddPostForward(postContentForward);
            }

            var userId = GetForwardedUserId(message);

            userPostFeed.AddUserId(userId);

            await _dbContext.NewsFeedCollection.InsertOneAsync(userPostFeed);
        }

        private UserPostFeed MappingUserPostFeedFrom(UserPostMessage message)
        {
            return new UserPostFeed
            {
                CreatedDate = message.CreatedAt,
                ChangedDate = message.UpdatedAt,
                PostContent = message.PostContent,
                PostId = message.PostId,
                PostType = message.PostType,
                PostedBy = message.CreatedBy,
                Url = message.Source.Url
            };
        }

        private CommunityPostFeed MappingCommunityPostFeedFrom(UserPostMessage message)
        {
            return new CommunityPostFeed()
            {
                CreatedDate = message.CreatedAt,
                ChangedDate = message.UpdatedAt,
                PostContent = message.PostContent,
                PostId = message.PostId,
                PostType = message.PostType,
                PostedBy = message.CreatedBy,
                Url = message.Source.Url,
                CommunityId = Guid.Parse(message.Source.Id.ToString()),
                CommunityName = message.Source.CommunityName,
                CommunityThumbnailUrl = message.Source.CommunityThumbnailUrl,
                Description = message.Source.CommunityDescription
            };
        }

        private async Task InsertIntoPost(UserPostMessage message)
        {
            var existedPost = await _dbContext
                .SyncedPostCollection
                .AsQueryable()
                .Where(p => p.PostId == message.PostId)
                .AnyAsync();

            if (existedPost)
            {
                _logger.LogError("UserPostCreated: Post already exists.");
                return;
            }

            var post = new Post(
                message.PostId,
                message.PostContent,
                message.Source.Url,
                message.SourceType,
                message.Visibility,
                message.CreatedBy,
                message.UpdatedBy,
                message.CreatedAt,
                message.UpdatedAt);

            post.SetPostType(message.PostType);

            await _dbContext.SyncedPostCollection.InsertOneAsync(post);
        }

        private Guid GetForwardedUserId(UserPostMessage message)
        {
            if (message.Source.UserId == null)
            {
                _logger.LogError("Can't find userId in message.");
                throw new GeneralException($"{nameof(UserPostCreatedConsumer)} can't find userId.");
            }

            return message.Source.UserId.Value;
        }
    }
}
