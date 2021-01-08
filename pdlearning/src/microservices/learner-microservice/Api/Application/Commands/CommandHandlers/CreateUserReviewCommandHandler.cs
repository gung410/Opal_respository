using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.Exceptions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class CreateUserReviewCommandHandler : BaseCommandHandler<CreateUserReviewCommand>
    {
        private readonly IWriteUserReviewLogic _writeUserReviewLogic;
        private readonly IRepository<UserReview> _userReviewRepository;
        private readonly IReadOnlyRepository<MyCourse> _readMyCourseRepository;
        private readonly IReadOnlyRepository<MyDigitalContent> _readMyDigitalContentRepository;

        public CreateUserReviewCommandHandler(
            IUserContext userContext,
            IUnitOfWorkManager unitOfWorkManager,
            IWriteUserReviewLogic writeUserReviewLogic,
            IRepository<UserReview> userReviewRepository,
            IReadOnlyRepository<MyCourse> readMyCourseRepository,
            IReadOnlyRepository<MyDigitalContent> readMyDigitalContentRepository) : base(unitOfWorkManager, userContext)
        {
            _userReviewRepository = userReviewRepository;
            _readMyCourseRepository = readMyCourseRepository;
            _writeUserReviewLogic = writeUserReviewLogic;
            _readMyDigitalContentRepository = readMyDigitalContentRepository;
        }

        protected override async Task HandleAsync(CreateUserReviewCommand command, CancellationToken cancellationToken)
        {
            if (command.ClassRunId != null)
            {
                await CreateReviewForClassRun(command);
            }
            else
            {
                await CreateReviewByItemType(command);
            }
        }

        private async Task CreateReviewByItemType(CreateUserReviewCommand command)
        {
            var existingUserReview = await _userReviewRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.ItemId == command.ItemId)
                .Where(p => p.ItemType == command.ItemType)
                .FirstOrDefaultAsync();

            if (existingUserReview != null)
            {
                throw new FeedbackExistedException();
            }

            switch (command.ItemType)
            {
                case ItemType.DigitalContent:
                    await CreateMyDigitalContentReview(command);
                    break;

                case ItemType.Course:
                    await CreateCourseReview(command);
                    break;
            }
        }

        private async Task CreateReviewForClassRun(CreateUserReviewCommand command)
        {
            var existingUserReview = await _userReviewRepository
                .GetAll()
                .Where(p => p.ItemType == command.ItemType)
                .Where(p => p.ClassRunId == command.ClassRunId)
                .Where(p => p.UserId == CurrentUserId)
                .FirstOrDefaultAsync();

            if (existingUserReview != null)
            {
                throw new FeedbackExistedException();
            }

            await InsertUserReview(command);
        }

        private async Task CreateMyDigitalContentReview(CreateUserReviewCommand command)
        {
            Expression<Func<MyDigitalContent, bool>> predicate = p => p.UserId == CurrentUserId && p.DigitalContentId == command.ItemId;

            await CreateUserReview<MyDigitalContent, MyDigitalContentStatus>(_readMyDigitalContentRepository, predicate, command);
        }

        private async Task CreateCourseReview(CreateUserReviewCommand command)
        {
            Expression<Func<MyCourse, bool>> predicate = p => p.UserId == CurrentUserId && p.CourseId == command.ItemId;

            await CreateUserReview<MyCourse, MyCourseStatus>(_readMyCourseRepository, predicate, command);
        }

        private async Task CreateUserReview<TSource, TEnum>(
            IReadOnlyRepository<TSource> repository,
            Expression<Func<TSource, bool>> predicate,
            CreateUserReviewCommand command)
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

            await InsertUserReview(command);
        }

        private Task InsertUserReview(CreateUserReviewCommand command)
        {
            var newUserReview = new UserReview
            {
                Id = command.Id,
                Rate = command.Rating,
                ItemId = command.ItemId,
                ItemType = command.ItemType,
                UserId = CurrentUserIdOrDefault,
                ClassRunId = command.ClassRunId,
                CreatedBy = CurrentUserIdOrDefault,
                UserFullName = command.UserFullName,
                CommentContent = command.CommentContent,
            };

            return _writeUserReviewLogic.Insert(newUserReview);
        }
    }
}
