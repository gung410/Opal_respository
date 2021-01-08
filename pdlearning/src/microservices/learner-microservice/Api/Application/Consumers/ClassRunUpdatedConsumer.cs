using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.Common;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.course.classrun.updated")]
    public class ClassRunUpdatedConsumer : ScopedOpalMessageConsumer<ClassRunChangeMessage>
    {
        private readonly IReadMyCourseShared _readMyCourseShared;
        private readonly ILogger<ClassRunUpdatedConsumer> _logger;
        private readonly IRepository<MyCourse> _myCourseRepository;
        private readonly IRepository<ClassRun> _classRunRepository;
        private readonly IReadMyClassRunShared _readMyClassRunShared;
        private readonly IWriteMyOutstandingTaskLogic _writeMyOutstandingTaskLogic;

        public ClassRunUpdatedConsumer(
            IReadMyCourseShared readMyCourseShared,
            ILogger<ClassRunUpdatedConsumer> logger,
            IRepository<ClassRun> classRunRepository,
            IRepository<MyCourse> myCourseRepository,
            IReadMyClassRunShared readMyClassRunShared,
            IWriteMyOutstandingTaskLogic writeMyOutstandingTaskLogic)
        {
            _logger = logger;
            _readMyCourseShared = readMyCourseShared;
            _classRunRepository = classRunRepository;
            _myCourseRepository = myCourseRepository;
            _readMyClassRunShared = readMyClassRunShared;
            _writeMyOutstandingTaskLogic = writeMyOutstandingTaskLogic;
        }

        public async Task InternalHandleAsync(ClassRunChangeMessage message)
        {
            var existingClassRun = await _classRunRepository
                .GetAll()
                .Where(p => p.Id == message.Id)
                .FirstOrDefaultAsync();

            if (existingClassRun == null)
            {
                return;
            }

            var classRunChangedDate = existingClassRun.ChangedDate.Value.ToUniversalTime();
            var messageCreatedDate = message.MessageCreatedDate.Value.ToUniversalTime();

            if (classRunChangedDate > messageCreatedDate)
            {
                // Implement Idempotent to avoid duplicate data come when the message can redeliver in RabbitMQ
                _logger.LogError(message: "The message comes from another system is out of date: {ClassRunChangedDate} {MessageCreatedDate}", classRunChangedDate, messageCreatedDate);
                return;
            }

            if (message.IsApprovedClassCancelled() || message.IsApprovedClassRescheduled())
            {
                // Update my course is based on the status of class.
                await UpdateMyCourseOnClassStatusChanged(message);

                // Delete outstanding task if the status of class is Cancelled.
                await DeleteMyOutstandingTaskOnClassCancelled(message, existingClassRun);
            }

            // Update DueDate of task is base on EndDateTime of class changed.
            await UpdateMyOutstandingTaskOnEndDateClassChanged(message, existingClassRun);

            // Update existing class run.
            await UpdateExistingClassRunFrom(message, existingClassRun);
        }

        /// <summary>
        /// To update display status of <see cref="MyCourse"/>
        /// Case 1: The class has cancelled.
        /// Case 2: The class has rescheduled.
        /// </summary>
        /// <param name="message">ClassRunChangeMessage listens from course module.</param>
        /// <returns>No results are returned.</returns>
        private async Task UpdateMyCourseOnClassStatusChanged(ClassRunChangeMessage message)
        {
            var isClassCancelled = message.IsApprovedClassCancelled();

            var inProgressRegistrations =
                await _readMyClassRunShared.GetInProgressRegistrationsByClassRunId(message.Id, isClassCancelled);

            var registrationIds = inProgressRegistrations
                .Select(p => p.RegistrationId)
                .ToList();

            if (!registrationIds.Any())
            {
                return;
            }

            var myCourses = await _readMyCourseShared
                .FilterByRegistrationIdsQuery(registrationIds)
                .ToListAsync();

            if (!myCourses.Any())
            {
                return;
            }

            myCourses.ForEach(myCourse =>
            {
                var registration = inProgressRegistrations
                    .FirstOrDefault(p => p.RegistrationId == myCourse.RegistrationId);

                if (registration == null)
                {
                    return;
                }

                if (message.IsApprovedClassCancelled())
                {
                    myCourse.SetDisplayStatus(
                        DisplayStatusMapper.MapFromClassRunCancelled(registration.RegistrationType));
                }
                else if (message.IsApprovedClassRescheduled())
                {
                    myCourse.SetDisplayStatus(
                        DisplayStatusMapper.MapFromClassRunRescheduled(registration.RegistrationType));
                }

                myCourse.ChangedDate = Clock.Now;
            });

            await _myCourseRepository.UpdateManyAsync(myCourses);
        }

        /// <summary>
        /// To delete outstanding course task if the class has cancelled.
        /// </summary>
        /// <param name="message">ClassRunChangeMessage listens from course module.</param>
        /// <param name="existingClassRun">ClassRun entity.</param>
        /// <returns>No results are returned.</returns>
        private async Task DeleteMyOutstandingTaskOnClassCancelled(ClassRunChangeMessage message, ClassRun existingClassRun)
        {
            if (!message.IsApprovedClassCancelled())
            {
                return;
            }

            await _writeMyOutstandingTaskLogic.DeleteManyCourseTask(existingClassRun);
        }

        /// <summary>
        /// To update due date of outstanding course task.
        /// </summary>
        /// <param name="message">ClassRunChangeMessage listens from course module.</param>
        /// <param name="existingClassRun">ClassRun entity.</param>
        /// <returns>No results are returned.</returns>
        private async Task UpdateMyOutstandingTaskOnEndDateClassChanged(ClassRunChangeMessage message, ClassRun existingClassRun)
        {
            if (!message.IsApprovedClassRescheduled())
            {
                return;
            }

            if (message.EndDateTime == existingClassRun.EndDateTime)
            {
                return;
            }

            await _writeMyOutstandingTaskLogic.UpdateDueDateOfCourseTask(existingClassRun, message.EndDateTime);
        }

        /// <summary>
        /// Mapping data from class run change message to my class run entity.
        /// </summary>
        /// <param name="message">Class run change message listen from course module.</param>
        /// <param name="existingClassRun">Class run entity.</param>
        private Task UpdateExistingClassRunFrom(ClassRunChangeMessage message, ClassRun existingClassRun)
        {
            existingClassRun.ClassTitle = message.ClassTitle;
            existingClassRun.ClassRunCode = message.ClassRunCode;
            existingClassRun.ApplicationEndDate = message.ApplicationEndDate;
            existingClassRun.ApplicationStartDate = message.ApplicationStartDate;
            existingClassRun.FacilitatorIds = message.FacilitatorIds;
            existingClassRun.CoFacilitatorIds = message.CoFacilitatorIds;
            existingClassRun.CourseId = message.CourseId;
            existingClassRun.EndDateTime = message.EndDateTime;
            existingClassRun.StartDateTime = message.StartDateTime;
            existingClassRun.PlanningStartTime = message.PlanningStartTime;
            existingClassRun.PlanningEndTime = message.PlanningEndTime;
            existingClassRun.MaxClassSize = message.MaxClassSize;
            existingClassRun.MinClassSize = message.MinClassSize;
            existingClassRun.CreatedBy = message.CreatedBy;
            existingClassRun.CreatedDate = message.CreatedDate;
            existingClassRun.Status = message.Status;
            existingClassRun.ChangedDate = message.ChangedDate;
            existingClassRun.ChangedBy = message.ChangedBy;
            existingClassRun.RescheduleStatus = message.RescheduleStatus;
            existingClassRun.RescheduleStartDateTime = message.RescheduleStartDateTime;
            existingClassRun.RescheduleEndDateTime = message.RescheduleEndDateTime;
            existingClassRun.CancellationStatus = message.CancellationStatus;
            existingClassRun.ClassRunVenueId = message.ClassRunVenueId;
            existingClassRun.ContentStatus = message.ContentStatus;
            existingClassRun.PublishedContentDate = message.PublishedContentDate;
            existingClassRun.SubmittedContentDate = message.SubmittedContentDate;
            existingClassRun.ApprovalContentDate = message.ApprovalContentDate;

            return _classRunRepository.UpdateAsync(existingClassRun);
        }
    }
}
