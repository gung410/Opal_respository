using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class UpdateUserReviewCommandHandler : BaseCommandHandler<UpdateUserReviewCommand>
    {
        private readonly IRepository<UserReview> _userReviewRepository;

        public UpdateUserReviewCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserReview> courseReviewRepository,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _userReviewRepository = courseReviewRepository;
        }

        protected override async Task HandleAsync(UpdateUserReviewCommand command, CancellationToken cancellationToken)
        {
            var userReview = await _userReviewRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.ItemId == command.ItemId)
                .Where(p => p.ItemType == command.ItemType)
                .FirstOrDefaultAsync(cancellationToken);

            if (userReview == null)
            {
                throw new EntityNotFoundException();
            }

            // Update user review information.
            userReview.Rate = command.Rating;
            userReview.ChangedDate = Clock.Now;
            userReview.ChangedBy = CurrentUserIdOrDefault;
            userReview.CommentContent = command.CommentContent;

            await _userReviewRepository.UpdateAsync(userReview);
        }
    }
}
