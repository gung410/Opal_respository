using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class DeleteUserReviewCommandHandler : BaseCommandHandler<DeleteUserReviewCommand>
    {
        private readonly IRepository<MyDigitalContent> _myDigitalContentRepository;
        private readonly IRepository<UserReview> _userReviewRepository;
        private readonly IRepository<MyCourse> _myCourseRepository;

        public DeleteUserReviewCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<MyDigitalContent> myDigitalContentRepository,
            IRepository<UserReview> userReviewRepository,
            IRepository<MyCourse> myCourseRepository,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _myDigitalContentRepository = myDigitalContentRepository;
            _userReviewRepository = userReviewRepository;
            _myCourseRepository = myCourseRepository;
        }

        protected override async Task HandleAsync(DeleteUserReviewCommand command, CancellationToken cancellationToken)
        {
            var userReview = await _userReviewRepository
                .GetAll()
                .Where(p => p.Id == command.Id)
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.ItemType == command.ItemType)
                .FirstOrDefaultAsync(cancellationToken);

            if (userReview == null)
            {
                throw new EntityNotFoundException();
            }

            await UpdateByItemType(command, userReview);
        }

        private async Task UpdateByItemType(DeleteUserReviewCommand command, UserReview userReview)
        {
            switch (command.ItemType)
            {
                case ItemType.DigitalContent:
                    await UpdateInProgress<MyDigitalContent, MyDigitalContentStatus>(
                        _myDigitalContentRepository,
                        p => p.UserId == CurrentUserId && p.DigitalContentId == userReview.ItemId,
                        userReview);
                    break;
                case ItemType.Course:
                    await UpdateInProgress<MyCourse, MyCourseStatus>(
                        _myCourseRepository,
                        p => p.UserId == CurrentUserId && p.CourseId == userReview.ItemId,
                        userReview);
                    break;
                default:
                    break;
            }
        }

        private async Task UpdateInProgress<TSource, TEnum>(
            IRepository<TSource> repository,
            Expression<Func<TSource, bool>> predicate,
            UserReview userReview)
            where TSource : class,
            IHasCompletionDate,
            IEntity<Guid>,
            IHasStatus<TEnum> where TEnum : Enum
        {
            var item = await repository.FirstOrDefaultAsync(predicate);
            if (item == null)
            {
                throw new EntityNotFoundException($"{nameof(TSource)} not found");
            }

            await _userReviewRepository.DeleteAsync(userReview);

            item.Status = (TEnum)Enum.Parse(typeof(TEnum), "InProgress");
            item.CompletedDate = null;
            await repository.UpdateAsync(item);
        }
    }
}
