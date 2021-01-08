using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.NewsFeed.Domain.Entities;
using Microservice.NewsFeed.Infrastructure;
using MongoDB.Bson;
using MongoDB.Driver;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.NewsFeed.Application.Commands.CommandHandlers
{
    public class MigrationUserCommandHandler : BaseThunderCommandHandler<MigrateUserCommand>
    {
        private readonly NewsFeedDbContext _dbContext;
        private readonly IUserContext _userContext;

        public MigrationUserCommandHandler(NewsFeedDbContext dbContext, IUserContext userContext)
        {
            _dbContext = dbContext;
            _userContext = userContext;
        }

        protected override async Task HandleAsync(MigrateUserCommand command, CancellationToken cancellationToken)
        {
            if (!_userContext.IsSysAdministrator())
            {
                throw new UnauthorizedAccessException("The user must be in role SysAdministrator!");
            }

            if (command.BatchSize <= 0)
            {
                throw new BusinessLogicException("Please specify the bath size.");
            }

            // We will use the default BsonDocument type instead of the Entity type
            // Because we don't define the UserTemporary collection on NewsFeedDbContext and UserTemporary entity to use.
            var userTemporaryCount = await _dbContext.Database
                .GetCollection<BsonDocument>("UserTemporary")
                .CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken: cancellationToken);

            var sortDefinition = new SortDefinitionBuilder<BsonDocument>().Descending(nameof(User.OriginalUserId));

            for (int i = 0; i < userTemporaryCount; i += command.BatchSize)
            {
                var usersTemporary = await _dbContext.Database
                    .GetCollection<BsonDocument>("UserTemporary")
                    .Find(FilterDefinition<BsonDocument>.Empty)
                    .Sort(sortDefinition)
                    .Skip(i)
                    .Limit(command.BatchSize)
                    .ToListAsync(cancellationToken);

                var users = usersTemporary.Select(p =>
                {
                    var originalUserId = p[nameof(User.OriginalUserId)].ToInt32();
                    var firstName = p[nameof(User.FirstName)].ToString();
                    var lastName = p[nameof(User.LastName)].ToString();
                    var email = p[nameof(User.Email)].ToString();
                    var extId = Guid.Parse(p[nameof(User.ExtId)].ToString());
                    var avatarUrl = p[nameof(User.AvatarUrl)].ToString();

                    var user = new User(
                        originalUserId,
                        firstName,
                        lastName,
                        email,
                        extId,
                        avatarUrl,
                        followers: new List<Guid>());

                    return user;
                }).ToList();

                await _dbContext.SyncedUserCollection.InsertManyAsync(users, cancellationToken: cancellationToken);
            }
        }
    }
}
