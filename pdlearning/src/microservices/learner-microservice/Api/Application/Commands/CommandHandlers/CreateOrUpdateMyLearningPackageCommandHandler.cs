using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Exceptions;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class CreateOrUpdateMyLearningPackageCommandHandler : BaseCommandHandler<CreateOrUpdateMyLearningPackageCommand>
    {
        private readonly IRepository<LectureInMyCourse> _lectureInMyCourseRepository;
        private readonly IRepository<MyDigitalContent> _myDigitalContentRepository;
        private readonly IRepository<MyLearningPackage> _myLearningPackageRepository;

        public CreateOrUpdateMyLearningPackageCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<MyLearningPackage> myLearningPackageRepository,
            IRepository<MyDigitalContent> myDigitalContentRepository,
            IRepository<LectureInMyCourse> lectureInMyCourseRepository,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _myLearningPackageRepository = myLearningPackageRepository;
            _myDigitalContentRepository = myDigitalContentRepository;
            _lectureInMyCourseRepository = lectureInMyCourseRepository;
        }

        protected override async Task HandleAsync(CreateOrUpdateMyLearningPackageCommand command, CancellationToken cancellationToken)
        {
            var query = _myLearningPackageRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId);

            // Course: command.MyLectureId.HasValue
            // Digital Content: command.MyDigitalContentId.HasValue
            if (command.MyLectureId.HasValue)
            {
                query = query.Where(p => p.MyLectureId == command.MyLectureId);
            }
            else if (command.MyDigitalContentId.HasValue)
            {
                query = query.Where(p => p.MyDigitalContentId == command.MyDigitalContentId);
            }

            query = query.OrderByDescending(p => p.CreatedDate);

            var myLearningPackage = await query.FirstOrDefaultAsync(cancellationToken);

            // Always create new one
            if (myLearningPackage == null)
            {
                await CreateMyLearningPackage(command);
            }

            // Process if existed record
            if (myLearningPackage != null)
            {
                await UpsertMyLearningPackage(myLearningPackage, command);
            }
        }

        private async Task CreateMyLearningPackage(CreateOrUpdateMyLearningPackageCommand command)
        {
            var myLearningPackage = new MyLearningPackage()
            {
                Id = Guid.NewGuid(),
                Type = command.Type,
                State = command.State,
                TimeSpan = command.TimeSpan,
                UserId = CurrentUserIdOrDefault,
                CreatedBy = CurrentUserIdOrDefault,
                LessonStatus = command.LessonStatus,
                SuccessStatus = command.SuccessStatus,
                CompletionStatus = command.CompletionStatus
            };

            if (command.MyLectureId.HasValue)
            {
                // Validate MyLectureId existed in DB
                await _lectureInMyCourseRepository.GetAsync(command.MyLectureId.Value);
                myLearningPackage.MyLectureId = command.MyLectureId.Value;
            }
            else if (command.MyDigitalContentId.HasValue)
            {
                // Validate MyDigitalContentId existed in DB
                await _myDigitalContentRepository.GetAsync(command.MyDigitalContentId.Value);
                myLearningPackage.MyDigitalContentId = command.MyDigitalContentId.Value;
            }

            await _myLearningPackageRepository.InsertAsync(myLearningPackage);
        }

        private async Task UpdateMyLearningPackage(MyLearningPackage myLearningPackage, CreateOrUpdateMyLearningPackageCommand command)
        {
            myLearningPackage.Type = command.Type;
            myLearningPackage.State = command.State;
            myLearningPackage.LessonStatus = command.LessonStatus;
            myLearningPackage.CompletionStatus = command.CompletionStatus;
            myLearningPackage.SuccessStatus = command.SuccessStatus;
            myLearningPackage.TimeSpan = command.TimeSpan;

            await _myLearningPackageRepository.UpdateAsync(myLearningPackage);
        }

        private async Task UpsertMyLearningPackage(MyLearningPackage myLearningPackage, CreateOrUpdateMyLearningPackageCommand command)
        {
            if (myLearningPackage.MyDigitalContentId.HasValue)
            {
                // Always update when MyDigitalContentId has value
                await UpdateMyLearningPackage(myLearningPackage, command);
            }
            else if (myLearningPackage.MyLectureId.HasValue)
            {
                var stateData = JsonConvert.DeserializeObject<ScormStateModel>(myLearningPackage.State);

                if (!stateData.CompletionStatus)
                {
                    // Lectures in Course: In case we must be created new one when latest record != completed
                    await CreateMyLearningPackage(command);
                }
                else
                {
                    throw new MyLearningPackageHasCompletedException();
                }
            }
            else
            {
                // When myLearningPackage excluded (MyDigitalContentId or MyLectureId)
                throw new ArgumentOutOfRangeException(nameof(myLearningPackage));
            }
        }
    }
}
