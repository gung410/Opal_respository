using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.NewsFeed.Application.Consumers.Messages.UserModification;
using Microservice.NewsFeed.Infrastructure;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using User = Microservice.NewsFeed.Domain.Entities.User;

namespace Microservice.NewsFeed.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.updated.employee")]
    public class UserUpdatedConsumer : OpalMessageConsumer<UserModificationMessage>
    {
        private readonly ILogger<UserUpdatedConsumer> _logger;
        private readonly NewsFeedDbContext _dbContext;

        public UserUpdatedConsumer(
            ILogger<UserUpdatedConsumer> logger,
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
                _logger.LogError("UpdateUser: ExtId cannot be null");
                return;
            }

            var filterByExtId = User.FilterByExtIdExpr(userData.Identity.ExtId);
            var user = await _dbContext
                .SyncedUserCollection
                .AsQueryable()
                .Where(filterByExtId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                // TODO Support sync data from SAM module to NewsFeed. It will be removed after release 3.0
                user = new User(
                    userData.Identity.Id,
                    userData.FirstName,
                    userData.LastName,
                    userData.EmailAddress,
                    userData.Identity.ExtId,
                    userData.JsonDynamicAttributes.AvatarUrl,
                    followers: new List<Guid>());

                await _dbContext.SyncedUserCollection.InsertOneAsync(user);

                return;
            }

            user.Update(
                userData.FirstName,
                userData.LastName,
                userData.EmailAddress,
                userData.JsonDynamicAttributes?.AvatarUrl);

            await _dbContext
                .SyncedUserCollection
                .ReplaceOneAsync(filterByExtId, user);
        }
    }
}
