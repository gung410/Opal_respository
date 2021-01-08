using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SaveClassRunCommandHandler : BaseCommandHandler<SaveClassRunCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly GetAggregatedClassRunSharedQuery _getAggregatedClassRunSharedQuery;
        private readonly ClassRunCudLogic _classRunCudLogic;

        public SaveClassRunCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUserContext userContext,
            ClassRunCudLogic classRunCudLogic,
            IAccessControlContext<CourseUser> accessControlContext,
            GetAggregatedClassRunSharedQuery getAggregatedClassRunSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _classRunCudLogic = classRunCudLogic;
            _getAggregatedClassRunSharedQuery = getAggregatedClassRunSharedQuery;
        }

        protected override async Task HandleAsync(SaveClassRunCommand command, CancellationToken cancellationToken)
        {
            var course = await _readCourseRepository.GetAsync(command.CourseId);
            EnsureBusinessLogicValid(course.ValidateNotArchived());

            if (command.IsCreate)
            {
                await CreateNew(command, cancellationToken);
            }
            else
            {
                await Update(command, cancellationToken);
            }
        }

        private async Task Update(SaveClassRunCommand command, CancellationToken cancellationToken)
        {
            var aggregatedClassRun = await _getAggregatedClassRunSharedQuery.ByClassRunId(command.Id);

            EnsureHasCudPermission(aggregatedClassRun.Course);

            EnsureBusinessLogicValid(aggregatedClassRun, p => p.ClassRun.CanUpdate(p.Course));

            SetDataForClassRunEntity(aggregatedClassRun.ClassRun, command);
            aggregatedClassRun.ClassRun.ChangedDate = Clock.Now;
            aggregatedClassRun.ClassRun.ChangedBy = CurrentUserId;

            await _classRunCudLogic.Update(aggregatedClassRun, cancellationToken);
        }

        private void EnsureHasCudPermission(CourseEntity course)
        {
            EnsureValidPermission(ClassRun.HasCudPermission(
                CurrentUserId,
                CurrentUserRoles,
                course,
                _readCourseRepository.GetHasAdminRightChecker(course, AccessControlContext),
                p => HasPermissionPrefix(p)));
        }

        private async Task CreateNew(SaveClassRunCommand command, CancellationToken cancellationToken)
        {
            var course = await _readCourseRepository.GetAsync(command.CourseId);
            EnsureHasCudPermission(course);

            var classRun = new ClassRun
            {
                Id = command.Id,
                Status = ClassRunStatus.Unpublished
            };

            SetDataForClassRunEntity(classRun, command);
            classRun.CreatedDate = Clock.Now;
            classRun.CreatedBy = CurrentUserIdOrDefault;
            classRun.ContentStatus = course.ContentStatus;

            await _classRunCudLogic.Insert(ClassRunAggregatedEntityModel.Create(classRun, course, new List<Session>()), cancellationToken);
        }

        private void SetDataForClassRunEntity(ClassRun classRun, SaveClassRunCommand command)
        {
            classRun.Id = command.Id;
            classRun.ClassTitle = command.ClassTitle;
            classRun.ApplicationEndDate = command.ApplicationEndDate;
            classRun.ApplicationStartDate = command.ApplicationStartDate;
            classRun.FacilitatorIds = command.FacilitatorIds;
            classRun.CoFacilitatorIds = command.CoFacilitatorIds;
            classRun.CourseId = command.CourseId;
            classRun.EndDateTime = command.EndDateTime;
            classRun.StartDateTime = command.StartDateTime;
            classRun.PlanningStartTime = command.PlanningStartTime;
            classRun.PlanningEndTime = command.PlanningEndTime;
            classRun.MaxClassSize = command.MaxClassSize;
            classRun.MinClassSize = command.MinClassSize;
        }
    }
}
