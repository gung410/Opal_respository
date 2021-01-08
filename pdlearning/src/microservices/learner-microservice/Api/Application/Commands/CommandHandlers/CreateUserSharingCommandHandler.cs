using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Exceptions;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class CreateUserSharingCommandHandler : BaseCommandHandler<CreateUserSharingCommand>
    {
        private readonly IRepository<UserSharing> _userSharingRepository;
        private readonly IRepository<UserSharingDetail> _userSharingDetailRepository;
        private readonly IRepository<LearnerLearningPath> _learningPathRepository;

        public CreateUserSharingCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserSharing> userSharingRepository,
            IRepository<UserSharingDetail> userSharingDetailRepository,
            IRepository<LearnerLearningPath> learningPathRepository,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _userSharingRepository = userSharingRepository;
            _userSharingDetailRepository = userSharingDetailRepository;
            _learningPathRepository = learningPathRepository;
        }

        protected override async Task HandleAsync(CreateUserSharingCommand command, CancellationToken cancellationToken)
        {
            if (command.IsLearningPath())
            {
                await CreateUserSharingForLearningPath(command, cancellationToken);
            }
            else if (command.IsPdo())
            {
                await CreateUserSharingForLearner(command);
            }
        }

        private async Task CreateUserSharingForLearningPath(CreateUserSharingCommand command, CancellationToken cancellationToken)
        {
            var existedLearningPath = await _learningPathRepository
                .GetAll()
                .Where(p => p.Id == command.ItemId)
                .AnyAsync(cancellationToken);

            if (!existedLearningPath)
            {
                throw new LearningPathNotFoundException();
            }

            var userSharing = new UserSharing
            {
                Id = command.Id,
                ItemType = command.ItemType,
                ItemId = command.ItemId,
                CreatedBy = CurrentUserIdOrDefault
            };

            await _userSharingRepository.InsertAsync(userSharing);

            var userSharingDetails = command.UsersShared
                .Select(c => new UserSharingDetail
                {
                    Id = Guid.NewGuid(),
                    UserId = c.UserId,
                    UserSharingId = command.Id,
                    CreatedBy = CurrentUserIdOrDefault
                })
                .ToList();

            await _userSharingDetailRepository.InsertManyAsync(userSharingDetails);
        }

        private async Task CreateUserSharingForLearner(CreateUserSharingCommand command)
        {
            var userSharing = new UserSharing
            {
                Id = command.Id,
                ItemId = command.ItemId,
                ItemType = command.ItemType,
                CreatedBy = CurrentUserIdOrDefault
            };

            await _userSharingRepository.InsertAsync(userSharing);

            var userSharingDetails = command.UsersShared
                .Select(c => new UserSharingDetail
                {
                    Id = Guid.NewGuid(),
                    UserId = c.UserId,
                    UserSharingId = command.Id,
                    CreatedBy = CurrentUserIdOrDefault
                })
                .ToList();

            await _userSharingDetailRepository.InsertManyAsync(userSharingDetails);
        }
    }
}
