using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class UpdateTrackingCommandHandler : BaseCommandHandler<UpdateTrackingCommand>
    {
        private readonly IRepository<UserLike> _userLikeRepository;

        public UpdateTrackingCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserLike> userLikeRepository,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _userLikeRepository = userLikeRepository;
        }

        protected override async Task HandleAsync(UpdateTrackingCommand command, CancellationToken cancellationToken)
        {
            switch (command.TrackingAction)
            {
                case Domain.ValueObject.LearningTrackingAction.View:
                    break;
                case Domain.ValueObject.LearningTrackingAction.DownloadContent:
                    break;
                case Domain.ValueObject.LearningTrackingAction.Like:
                    await Like(command, cancellationToken);
                    break;
                case Domain.ValueObject.LearningTrackingAction.Share:
                    break;
                default:
                    break;
            }
        }

        private async Task Like(UpdateTrackingCommand command, CancellationToken cancellationToken)
        {
            var existedUserLike = await _userLikeRepository
                .GetAll()
                .Where(user => user.ItemId == command.ItemId)
                .Where(user => user.ItemType == command.ItemType)
                .Where(user => user.CreatedBy == CurrentUserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existedUserLike != null)
            {
                if (!command.IsLike)
                {
                    await _userLikeRepository.DeleteAsync(existedUserLike);
                }
            }
            else
            {
                var userLike = new UserLike
                {
                    Id = Guid.NewGuid(),
                    ItemId = command.ItemId,
                    ItemType = command.ItemType,
                    CreatedBy = CurrentUserIdOrDefault
                };

                await _userLikeRepository.InsertAsync(userLike);
            }
        }
    }
}
