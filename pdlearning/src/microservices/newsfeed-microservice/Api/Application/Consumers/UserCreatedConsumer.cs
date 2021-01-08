using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.NewsFeed.Application.Consumers.Messages.UserModification;
using Microservice.NewsFeed.Infrastructure;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Microservice.NewsFeed.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.created.employee")]
    public class UserCreatedConsumer : OpalMessageConsumer<UserModificationMessage>
    {
        private readonly ILogger<UserCreatedConsumer> _logger;
        private readonly NewsFeedDbContext _dbContext;

        public UserCreatedConsumer(
            ILogger<UserCreatedConsumer> logger,
            NewsFeedDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(UserModificationMessage message)
        {
            var userData = message.UserData;

            if (userData.Identity?.ExtId == null)
            {
                _logger.LogError("InsertUser: ExtId cannot be null");
                return;
            }

            var existedUser = await _dbContext
                .SyncedUserCollection
                .AsQueryable()
                .Where(Domain.Entities.User.FilterByExtIdExpr(userData.Identity.ExtId))
                .AnyAsync();

            if (existedUser)
            {
                _logger.LogError("InsertUser: User already exists.");
                return;
            }

            var user = new Domain.Entities.User(
                userData.Identity.Id,
                userData.FirstName,
                userData.LastName,
                userData.EmailAddress,
                userData.Identity.ExtId,
                userData.JsonDynamicAttributes.AvatarUrl,
                followers: new List<Guid>());

            await _dbContext.SyncedUserCollection.InsertOneAsync(user);
        }
    }
}
