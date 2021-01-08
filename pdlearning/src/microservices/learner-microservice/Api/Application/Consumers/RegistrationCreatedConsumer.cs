using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.SharedLogic;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.course.registration.created")]
    public class RegistrationCreatedConsumer : InboxSupportConsumer<RegistrationChangeMessage>
    {
        private readonly IReadMyCourseShared _readMyCourseShared;
        private readonly IRepository<MyCourse> _myCourseRepository;
        private readonly IReadMyClassRunShared _readMyClassRunShared;
        private readonly IRepository<MyClassRun> _myClassRunRepository;
        private readonly IWriteMyOutstandingTaskLogic _myOutstandingTaskCudLogic;

        public RegistrationCreatedConsumer(
            IReadMyCourseShared readMyCourseShared,
            IRepository<MyCourse> myCourseRepository,
            IReadMyClassRunShared readMyClassRunShared,
            IRepository<MyClassRun> myClassRunRepository,
            IWriteMyOutstandingTaskLogic myOutstandingTaskCudLogic)
        {
            _readMyCourseShared = readMyCourseShared;
            _myCourseRepository = myCourseRepository;
            _readMyClassRunShared = readMyClassRunShared;
            _myClassRunRepository = myClassRunRepository;
            _myOutstandingTaskCudLogic = myOutstandingTaskCudLogic;
        }

        public async Task InternalHandleAsync(RegistrationChangeMessage message)
        {
            var existedRegistration = await _myClassRunRepository
                .GetAll()
                .Where(p => p.RegistrationId == message.Id)
                .AnyAsync();

            if (existedRegistration)
            {
                return;
            }

            // Bypass MyCourse display status updates if the the class is full or conflicted when adding participant.
            if (!message.IsClassFullOrConflict())
            {
                await UpdateMyCourseOnWorkflowChanged(message);
            }

            // Only update registrationId if the the class is full or conflicted when adding participant.
            else if (message.IsClassFullOrConflict())
            {
                await UpdateMyCourseOnClassFullOrConflicted(message);
            }

            // Insert a new class run.
            await InsertMyClassRun(message);
        }

        /// <summary>
        /// To create or update <see cref="MyCourse"/>.
        /// </summary>
        /// <param name="message">RegistrationMessage listens from course module.</param>
        /// <returns>No results to returned.</returns>
        private async Task UpdateMyCourseOnWorkflowChanged(RegistrationChangeMessage message)
        {
            var notExpiredRegistrations =
                await _readMyClassRunShared.GetNotExpiredRegistrations(
                    message.UserId,
                    courseIds: new List<Guid> { message.CourseId });

            var existingMyCourse =
                await _readMyCourseShared.GetByUserIdAndCourseId(message.CourseId, message.UserId);

            var finishedRegistration = notExpiredRegistrations.FirstOrDefault(p => p.IsFinishedLearning());

            var isLearningCompleted =
                finishedRegistration != null
                && finishedRegistration.IsFinishedLearning()
                && existingMyCourse.IsFinishedLearning();

            if (existingMyCourse == null || isLearningCompleted)
            {
                // MyCourse is created:
                // Case 1. User not registered.
                // Case 2. The user's learning Completed or Failed.
                await InsertMyCourse(message);
            }
            else
            {
                var inProgressRegistration = notExpiredRegistrations
                    .FirstOrDefault(p => p.IsInProgress() && !p.IsFinishedLearning());

                // To update display status of MyCourse
                await UpdateMyCourseOnStatusChanged(inProgressRegistration, message, existingMyCourse);
            }
        }

        /// <summary>
        /// To update display status of <see cref="MyCourse"/>
        /// Case 1: User's request to change class confirmed by administrator.
        /// Case 2: User withdrew from the class.
        /// Case 3: User's registration rejected.
        /// Case 4: User's registration expired.
        /// </summary>
        /// <param name="inProgressRegistration">MyClassRun entity.</param>
        /// <param name="message">RegistrationChangeMessage listens from course module.</param>
        /// <param name="existingMyCourse">MyCourse entity.</param>
        private async Task UpdateMyCourseOnStatusChanged(MyClassRun inProgressRegistration, RegistrationChangeMessage message, MyCourse existingMyCourse)
        {
            // If the request change class was confirmed by course administrator.
            // The system will create a new registration.
            // But so that we can set the correct display status for my course.
            // We need to check if the current registration with change class status is Approved.
            // If yes we set display status by change class status.
            if (!message.IsWaitListStatuses()
                && inProgressRegistration != null
                && inProgressRegistration.IsApprovedClassChange())
            {
                existingMyCourse.SetClassRunChangeDisplayStatus(ClassRunChangeStatus.ConfirmedByCA);
            }
            else
            {
                existingMyCourse.SetDisplayStatus(
                    message.Status,
                    message.RegistrationType);
            }

            existingMyCourse.MyRegistrationStatus = message.Status;
            existingMyCourse.MyWithdrawalStatus = message.WithdrawalStatus;
            existingMyCourse.ChangedDate = message.ChangedDate;
            existingMyCourse.SetRegistrationId(message.Id);

            await _myCourseRepository.UpdateAsync(existingMyCourse);
        }

        /// <summary>
        /// To map the data from <see cref="RegistrationChangeMessage"/> to <see cref="MyCourse"/> entity.
        /// </summary>
        /// <param name="message">RegistrationChangeMessage listens from course module.</param>
        private async Task InsertMyCourse(RegistrationChangeMessage message)
        {
            var now = Clock.Now;
            var newMyCourse = new MyCourse
            {
                Id = Guid.NewGuid(),
                CourseId = message.CourseId,
                UserId = message.UserId,
                CreatedBy = message.UserId,
                Status = MyCourseStatus.NotStarted,
                CourseType = LearningCourseType.FaceToFace,
                MyRegistrationStatus = message.Status,
                MyWithdrawalStatus = message.WithdrawalStatus,
                PostCourseEvaluationFormCompleted = message.PostCourseEvaluationFormCompleted,
                ProgressMeasure = 0,
                StartDate = now,
                LastLogin = now,
            };

            newMyCourse.SetRegistrationId(message.Id);
            newMyCourse.SetDisplayStatus(
                message.Status,
                message.RegistrationType);

            await _myCourseRepository.InsertAsync(newMyCourse);
        }

        /// <summary>
        /// To map the data from <see cref="RegistrationChangeMessage"/> to <see cref="MyClassRun"/> entity.
        /// </summary>
        /// <param name="message">RegistrationChangeMessage listens from course module.</param>
        /// <returns>No results are returned.</returns>
        private async Task InsertMyClassRun(RegistrationChangeMessage message)
        {
            var myClassRun = new MyClassRun
            {
                Id = Guid.NewGuid(),
                CourseId = message.CourseId,
                UserId = message.UserId,
                ClassRunId = message.ClassRunId,
                Status = message.Status,
                WithdrawalStatus = message.WithdrawalStatus,
                RegistrationId = message.Id,
                ChangedDate = message.ChangedDate,
                RegistrationType = message.RegistrationType,
                LearningStatus = message.LearningStatus,
                PostCourseEvaluationFormCompleted = message.PostCourseEvaluationFormCompleted,
                IsExpired = message.IsExpired,
                LearningContentProgress = message.LearningContentProgress
            };

            await _myClassRunRepository.InsertAsync(myClassRun);

            // Add outstanding course task.
            if (!message.CanInsertOutstandingTask())
            {
                return;
            }

            // Add outstanding task when the course administrator has confirmed registration
            // or the user has accepted the offer from course administrator.
            await _myOutstandingTaskCudLogic.InsertCourseTask(myClassRun);
        }

        /// <summary>
        /// To update registrationId if the the class is full or conflicted when adding participant.
        /// </summary>
        /// <param name="message">RegistrationChangeMessage listens from course module.</param>
        /// <returns>No results are returned.</returns>
        private async Task UpdateMyCourseOnClassFullOrConflicted(RegistrationChangeMessage message)
        {
            var existingMyCourse = await _readMyCourseShared
                .GetByUserIdAndCourseId(message.CourseId, message.UserId);

            if (existingMyCourse == null)
            {
                return;
            }

            existingMyCourse.SetRegistrationId(message.Id);
        }
    }
}
