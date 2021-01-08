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
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.course.registration.updated")]
    public class RegistrationUpdatedConsumer : InboxSupportConsumer<RegistrationChangeMessage>
    {
        private readonly IReadMyCourseShared _readMyCourseShared;
        private readonly IRepository<MyCourse> _myCourseRepository;
        private readonly IReadMyClassRunShared _readMyClassRunShared;
        private readonly ILogger<RegistrationUpdatedConsumer> _logger;
        private readonly IRepository<MyClassRun> _myClassRunRepository;
        private readonly IWriteMyOutstandingTaskLogic _myOutstandingTaskCudLogic;

        public RegistrationUpdatedConsumer(
            IReadMyCourseShared readMyCourseShared,
            IRepository<MyCourse> myCourseRepository,
            IReadMyClassRunShared readMyClassRunShared,
            ILogger<RegistrationUpdatedConsumer> logger,
            IRepository<MyClassRun> myClassRunRepository,
            IWriteMyOutstandingTaskLogic myOutstandingTaskCudLogic)
        {
            _logger = logger;
            _readMyCourseShared = readMyCourseShared;
            _myCourseRepository = myCourseRepository;
            _myClassRunRepository = myClassRunRepository;
            _readMyClassRunShared = readMyClassRunShared;
            _myOutstandingTaskCudLogic = myOutstandingTaskCudLogic;
        }

        public async Task InternalHandleAsync(RegistrationChangeMessage message)
        {
            var existingMyClassRun = await _myClassRunRepository
                .GetAll()
                .Where(p => p.RegistrationId == message.Id)
                .FirstOrDefaultAsync();

            if (existingMyClassRun == null)
            {
                _logger.LogError(message: "MyClassRun not found with registrationId: {Id}", message.Id);
                return;
            }

            // Update MyCourse is base on the latest user perform on the workflow.
            await UpdateMyCourseOnWorkflowChanged(message, existingMyClassRun);

            // Need to update MyClassRun after MyCourse update to compare the change of statues.
            await UpdateExistingMyClassRunFrom(existingMyClassRun, message);
        }

        /// <summary>
        /// To update or add <see cref="MyCourse"/> is based on user performs on workflow.
        /// </summary>
        /// <param name="message">RegistrationMessage listens from course module.</param>
        /// <param name="existingMyClassRun">MyClassRun entity.</param>
        /// <returns>No results to returned.</returns>
        private async Task UpdateMyCourseOnWorkflowChanged(RegistrationChangeMessage message, MyClassRun existingMyClassRun)
        {
            var existingMyCourse = await _readMyCourseShared
                .FilterByRegistrationIdsQuery(new List<Guid>() { message.Id })
                .FirstOrDefaultAsync();

            if (message.IsConfirmedClassChange())
            {
                // We need to ensure that a new registry has been inserted
                // after the class change request was confirmed by the course administrator.
                for (int i = 1; i < 3; i++)
                {
                    var isInserted = await _readMyClassRunShared
                        .EnsureMyClassRunInserted(message.UserId, message.CourseId);

                    if (isInserted)
                    {
                        break;
                    }

                    // The task delay should be set here to avoid fast execution loop
                    // while the new my class run has not been inserted.
                    await Task.Delay(500);
                }
            }
            else if (existingMyCourse == null && existingMyClassRun.IsClassFullOrConflict())
            {
                // The first, course administrator adding participant but the class is full or conflict,
                // then the course administrator re-adds the user to the participant when the class has some slots
                await InsertMyCourse(message);
            }
            else if (existingMyCourse != null)
            {
                await UpdateMyCourseOnInProgressRegistration(message, existingMyClassRun, existingMyCourse);
            }
        }

        /// <summary>
        /// Add outstanding task when the registration has been confirmed by course administrator
        /// or the user has been accepted the offer from the course administrator.
        /// Remove outstanding task when the user has been withdrawn from the class
        /// or class change request has been confirmed.
        /// </summary>
        /// <param name="message">RegistrationMessage listens from course module.</param>
        /// <param name="existingMyClassRun">MyClassRun entity.</param>
        /// <returns>No results are returned.</returns>
        private Task InsertOrDeleteMyOutstandingTask(RegistrationChangeMessage message, MyClassRun existingMyClassRun)
        {
            // Remove outstanding task.
            if (message.CanDeleteOutstandingTask())
            {
                return _myOutstandingTaskCudLogic.DeleteCourseTask(existingMyClassRun);
            }

            // Add outstanding task.
            if (message.CanInsertOutstandingTask() && existingMyClassRun.IsPending())
            {
                return _myOutstandingTaskCudLogic.InsertCourseTask(existingMyClassRun);
            }

            return Task.CompletedTask;
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
        /// To update the <see cref="MyCourse"/> is based on existing my course, existing my class run and current registration message.
        /// </summary>
        /// <param name="message">RegistrationMessage listens from course module.</param>
        /// <param name="existingMyClassRun">MyClassRun entity.</param>
        /// <param name="existingMyCourse">MyCourse entity.</param>
        /// <returns>No results are returned.</returns>
        private async Task UpdateMyCourseOnInProgressRegistration(RegistrationChangeMessage message, MyClassRun existingMyClassRun, MyCourse existingMyCourse)
        {
            // Get the list of MyClassRun.
            var inProgressRegistrations = await _readMyClassRunShared
                .GetInProgressRegistrations(message.UserId, message.CourseId);

            // Update status of MyCourse.
            UpdateMyCourseOnLearningStatusChanged(existingMyCourse, message);

            // There are two registrations in progress.
            if (inProgressRegistrations.Count > 1)
            {
                UpdateMyCourseOnRegistrationDuplicated(existingMyCourse, inProgressRegistrations, message);
            }

            // There is one registration in-progress
            // There is no registration in progress if the class is full or conflicted.
            else if (inProgressRegistrations.Count == 1 || existingMyClassRun.IsClassFullOrConflict())
            {
                UpdateMyCourseOnCurrentRegistration(existingMyCourse, existingMyClassRun, message);
            }

            existingMyCourse.ChangedDate = message.ChangedDate;
            existingMyCourse.MyWithdrawalStatus = message.WithdrawalStatus;

            await _myCourseRepository.UpdateAsync(existingMyCourse);
        }

        /// <summary>
        /// To update the learning status of <see cref="MyCourse"/>.
        /// </summary>
        /// <param name="existingMyCourse">MyCourse entity.</param>
        /// <param name="registrationMessage">RegistrationMessage listens from course module.</param>
        private void UpdateMyCourseOnLearningStatusChanged(MyCourse existingMyCourse, RegistrationChangeMessage registrationMessage)
        {
            if (registrationMessage.IsFinishedLearning()
                && registrationMessage.LearningStatus != null
                && existingMyCourse.Status != (MyCourseStatus)registrationMessage.LearningStatus)
            {
                if (registrationMessage.LearningStatus != null)
                {
                    existingMyCourse.Status = (MyCourseStatus)registrationMessage.LearningStatus;

                    if (registrationMessage.IsLearningCompleted())
                    {
                        existingMyCourse.CompletedDate = registrationMessage.LearningCompletedDate;
                    }
                }
            }

            // TODO: need to check this logic because the PostCourseEvaluationFormCompleted is no longer used.
            if (registrationMessage.PostCourseEvaluationFormCompleted != existingMyCourse.PostCourseEvaluationFormCompleted)
            {
                existingMyCourse.PostCourseEvaluationFormCompleted = registrationMessage.PostCourseEvaluationFormCompleted;
            }
        }

        /// <summary>
        /// To update the display status of <see cref="MyCourse"/> with two registrations that are manual and nominated.
        /// 1. Manual registration (created by User).
        /// 2. Nomination registration (created by administrator and high priority).
        /// </summary>
        /// <param name="existingMyCourse">MyCourse entity.</param>
        /// <param name="myClassRunsInProgress">MyClassRun list.</param>
        /// <param name="message">RegistrationMessage listens from course module.</param>
        private void UpdateMyCourseOnRegistrationDuplicated(
            MyCourse existingMyCourse,
            List<MyClassRun> myClassRunsInProgress,
            RegistrationChangeMessage message)
        {
            if (message.IsRejectStatuses())
            {
                UpdateMyCourseOnRejectStatusChanged(
                    existingMyCourse,
                    message.Id,
                    myClassRunsInProgress);
            }
            else if (message.IsWaitListStatuses())
            {
                UpdateMyCourseOnWaitListStatusChanged(
                    existingMyCourse,
                    message);
            }
            else if (message.IsConfirmedByCourseAdministrator())
            {
                UpdateMyCourseOnCourseAdministratorConfirmed(
                    existingMyCourse,
                    message,
                    myClassRunsInProgress);
            }
        }

        /// <summary>
        /// To update the display status of <see cref="MyCourse"/> with two registrations that are manual and nominated.
        /// If one registration is rejected by course administrator or approving officer,
        /// MyCourse display status will be based on one remaining registration.
        /// </summary>
        /// <param name="existingMyCourse">MyCourse entity.</param>
        /// <param name="registrationId">The identifier registration.</param>
        /// <param name="myClassRunsInProgress">MyClassRun list.</param>
        private void UpdateMyCourseOnRejectStatusChanged(MyCourse existingMyCourse, Guid registrationId, List<MyClassRun> myClassRunsInProgress)
        {
            var remainingMyClassRun = myClassRunsInProgress
                .FirstOrDefault(p => p.RegistrationId != registrationId);

            if (remainingMyClassRun == null)
            {
                return;
            }

            existingMyCourse.SetDisplayStatus(
                remainingMyClassRun.Status,
                remainingMyClassRun.RegistrationType);

            existingMyCourse.SetMyRegistrationStatus(remainingMyClassRun.Status);
            existingMyCourse.SetRegistrationId(remainingMyClassRun.RegistrationId);
        }

        /// <summary>
        /// To update the display status of <see cref="MyCourse"/> with two registrations that are manual and nominated.
        /// If any of the registrations are confirmed by course administrator,
        /// MyCourse display status will be based on that registration.
        /// </summary>
        /// <param name="existingMyCourse">MyCourse entity.</param>
        /// <param name="message">RegistrationMessage listens from course module.</param>
        /// <param name="myClassRunsInProgress">MyClassRun list.</param>
        private void UpdateMyCourseOnCourseAdministratorConfirmed(
            MyCourse existingMyCourse,
            RegistrationChangeMessage message,
            List<MyClassRun> myClassRunsInProgress)
        {
            var myClassRunInProgress = myClassRunsInProgress
                .FirstOrDefault(p => p.RegistrationId == message.Id);

            if (myClassRunInProgress == null)
            {
                return;
            }

            SetDisplayStatus(existingMyCourse, myClassRunInProgress, message);
            existingMyCourse.SetMyRegistrationStatus(message.Status);
            existingMyCourse.SetRegistrationId(message.Id);
        }

        /// <summary>
        /// To update display status is base on current registration message.
        /// </summary>
        /// <param name="existingMyCourse">MyCourse entity.</param>
        /// <param name="existingMyClassRun">MyClassRun entity.</param>
        /// <param name="message">RegistrationMessage listens from course module.</param>
        private void UpdateMyCourseOnCurrentRegistration(MyCourse existingMyCourse, MyClassRun existingMyClassRun, RegistrationChangeMessage message)
        {
            existingMyCourse.SetRegistrationId(message.Id);
            existingMyCourse.SetMyRegistrationStatus(message.Status);
            SetDisplayStatus(existingMyCourse, existingMyClassRun, message);
        }

        /// <summary>
        /// To update the display status of <see cref="MyCourse"/> with two registrations that are manual and nominated.
        /// Because nomination is higher priority than manual registration
        /// So display status will be based on the <see cref="RegistrationStatus"/> nominated.
        /// </summary>
        /// <param name="existingMyCourse">My course entity.</param>
        /// <param name="registrationMessage">RegistrationMessage listens from course module.</param>
        private void UpdateMyCourseOnWaitListStatusChanged(
            MyCourse existingMyCourse,
            RegistrationChangeMessage registrationMessage)
        {
            // Return if the registration is manual.
            if (registrationMessage.IsManualRegistration())
            {
                return;
            }

            existingMyCourse.SetDisplayStatus(
                registrationMessage.Status,
                registrationMessage.RegistrationType);

            existingMyCourse.SetMyRegistrationStatus(registrationMessage.Status);
            existingMyCourse.SetRegistrationId(registrationMessage.Id);
        }

        /// <summary>
        /// To set the latest status that user has performed on workflow
        /// (the latest status could be
        /// <see cref="RegistrationStatus"/>
        /// or <see cref="WithdrawalStatus"/>
        /// or <see cref="ClassRunChangeStatus"/>).
        /// </summary>
        /// <param name="existingMyCourse">MyCourse entity.</param>
        /// <param name="existingMyClassRun">MyClassRun entity.</param>
        /// <param name="message">RegistrationMessage listens from course module.</param>
        private void SetDisplayStatus(MyCourse existingMyCourse, MyClassRun existingMyClassRun, RegistrationChangeMessage message)
        {
            if (existingMyClassRun.HasChangeClassStatusChanged(message.ClassRunChangeStatus))
            {
                // Class run change status changes.
                existingMyCourse.SetClassRunChangeDisplayStatus(message.ClassRunChangeStatus);
            }
            else if (existingMyClassRun.HasWithdrawStatusChanged(message.WithdrawalStatus))
            {
                // Withdrawal status changes.
                existingMyCourse.SetWithdrawalDisplayStatus(message.WithdrawalStatus);
            }
            else if (existingMyClassRun.HasRegistrationStatusChanged(message.Status)
                     || existingMyClassRun.HasRegistrationTypeChanged(message.RegistrationType))
            {
                // Main registration status changes.
                existingMyCourse.SetDisplayStatus(
                    message.Status,
                    message.RegistrationType);
            }
        }

        /// <summary>
        /// To map the data from <see cref="RegistrationChangeMessage"/> to <see cref="MyClassRun"/> entity.
        /// </summary>
        /// <param name="existingMyClassRun">MyClassRun entity.</param>
        /// <param name="message">RegistrationMessage listens from course module.</param>
        private async Task UpdateExistingMyClassRunFrom(MyClassRun existingMyClassRun, RegistrationChangeMessage message)
        {
            existingMyClassRun.Status = message.Status;
            existingMyClassRun.WithdrawalStatus = message.WithdrawalStatus;
            existingMyClassRun.ChangedDate = message.ChangedDate;
            existingMyClassRun.AdministratedBy = message.AdministratedBy;
            existingMyClassRun.ChangedBy = message.ChangedBy;
            existingMyClassRun.ClassRunChangeStatus = message.ClassRunChangeStatus;
            existingMyClassRun.ClassRunChangeRequestedDate = message.ClassRunChangeRequestedDate;
            existingMyClassRun.ClassRunChangeId = message.ClassRunChangeId;
            existingMyClassRun.LearningStatus = message.LearningStatus;
            existingMyClassRun.RegistrationType = message.RegistrationType;
            existingMyClassRun.PostCourseEvaluationFormCompleted = message.PostCourseEvaluationFormCompleted;
            existingMyClassRun.IsExpired = message.IsExpired;
            existingMyClassRun.LearningContentProgress = message.LearningContentProgress;

            await _myClassRunRepository.UpdateAsync(existingMyClassRun);

            // Add or remove outstanding course task
            // is based on RegistrationStatus or WithdrawStatus or ClassRunChangeStatus changed.
            await InsertOrDeleteMyOutstandingTask(message, existingMyClassRun);
        }
    }
}
