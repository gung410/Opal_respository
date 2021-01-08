using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Application.SharedQueries.GetHaveRegistrationsClassRuns;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ChangeClassRunCancellationStatusCommandHandler : BaseCommandHandler<ChangeClassRunCancellationStatusCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly SendClassRunNotificationLogic _sendClassRunNotificationLogic;
        private readonly GetHaveRegistrationsClassRunsSharedQuery _getHaveRegistrationsClassRunsSharedQuery;
        private readonly GetAggregatedClassRunSharedQuery _aggregatedClassRunSharedQuery;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly ClassRunCudLogic _classRunCudLogic;
        private readonly RegistrationCudLogic _registrationCudLogic;
        private readonly SetRegistrationsExpiredLogic _setRegistrationsExpiredLogic;

        public ChangeClassRunCancellationStatusCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            SendClassRunNotificationLogic sendClassRunNotificationLogic,
            ClassRunCudLogic classRunCudLogic,
            RegistrationCudLogic registrationCudLogic,
            GetHaveRegistrationsClassRunsSharedQuery getHaveRegistrationsClassRunsSharedQuery,
            WebAppLinkBuilder webAppLinkBuilder,
            SetRegistrationsExpiredLogic setRegistrationsExpiredLogic,
            GetAggregatedClassRunSharedQuery aggregatedClassRunSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _sendClassRunNotificationLogic = sendClassRunNotificationLogic;
            _getHaveRegistrationsClassRunsSharedQuery = getHaveRegistrationsClassRunsSharedQuery;
            _classRunCudLogic = classRunCudLogic;
            _registrationCudLogic = registrationCudLogic;
            _webAppLinkBuilder = webAppLinkBuilder;
            _aggregatedClassRunSharedQuery = aggregatedClassRunSharedQuery;
            _setRegistrationsExpiredLogic = setRegistrationsExpiredLogic;
        }

        protected override async Task HandleAsync(ChangeClassRunCancellationStatusCommand command, CancellationToken cancellationToken)
        {
            var aggregatedClassRuns = await _aggregatedClassRunSharedQuery.ByClassRunIds(command.Ids, true, cancellationToken);
            var classRuns = aggregatedClassRuns.Select(p => p.ClassRun).ToList();

            switch (command.CancellationStatus)
            {
                case ClassRunCancellationStatus.PendingApproval:
                    EnsureCanSubmitForApproving(aggregatedClassRuns);
                    ChangeStatusForPendingApproval(classRuns, command);
                    break;

                case ClassRunCancellationStatus.Approved:
                    EnsureCanApproving(aggregatedClassRuns);
                    await ChangeStatusForApproved(classRuns, command, cancellationToken);
                    break;

                case ClassRunCancellationStatus.Rejected:
                    EnsureCanApproving(aggregatedClassRuns);
                    ChangeStatusForRejected(classRuns, command);

                    break;
            }

            await _classRunCudLogic.UpdateMany(aggregatedClassRuns, cancellationToken);

            if (command.CancellationStatus == ClassRunCancellationStatus.Approved)
            {
                await SendNotificationWhenApproveCancellingClassRun(classRuns, cancellationToken);
            }
        }

        private async Task SendNotificationWhenApproveCancellingClassRun(
            List<ClassRun> classRuns,
            CancellationToken cancellationToken)
        {
            var haveRegistrationsClassRuns = await _getHaveRegistrationsClassRunsSharedQuery.ByClassRuns(classRuns, cancellationToken);
            var classRunIdToRegistrationsDic = haveRegistrationsClassRuns.ToDictionary(p => p.ClassRun.Id, p => p.Registrations);

            if (!haveRegistrationsClassRuns.Any())
            {
                return;
            }

            await _sendClassRunNotificationLogic.ByClassRuns(
                haveRegistrationsClassRuns.Select(p => p.ClassRun).ToList(),
                (classRun, course) => new ApprovedCancellationClassRunNotifyLearnerEvent(
                    CurrentUserIdOrDefault,
                    new ApprovedCancellationClassRunNotifyLearnerPayload
                    {
                        CourseName = course.CourseName,
                        ClassTitle = classRun.ClassTitle,
                        CourseCode = course.CourseCode,
                        ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(course.Id),
                        ObjectType = TodoEventPayloadObjectTypesConstant.Course,
                        ObjectId = course.Id
                    },
                    classRunIdToRegistrationsDic[classRun.Id].Select(x => x.UserId).ToList()),
                cancellationToken);
        }

        private void ChangeStatusForPendingApproval(List<ClassRun> classRuns, ChangeClassRunCancellationStatusCommand command)
        {
            classRuns.ForEach(classRun =>
            {
                classRun.CancellationStatus = command.CancellationStatus;
                classRun.ChangedBy = CurrentUserId;
                classRun.ChangedDate = Clock.Now;
            });
        }

        private async Task ChangeStatusForApproved(List<ClassRun> classRuns, ChangeClassRunCancellationStatusCommand command, CancellationToken cancellationToken)
        {
            classRuns.ForEach(classRun =>
            {
                classRun.CancellationStatus = command.CancellationStatus;
                classRun.ChangedBy = CurrentUserId;
                classRun.ChangedDate = Clock.Now;
                classRun.Status = ClassRunStatus.Cancelled;
            });

            var classrunIds = classRuns.Select(x => x.Id).ToList();
            var registrations = await _setRegistrationsExpiredLogic.Execute(classrunIds, CurrentUserIdOrDefault, true, cancellationToken);

            await _registrationCudLogic.UpdateMany(registrations, cancellationToken);
        }

        private void ChangeStatusForRejected(List<ClassRun> classRuns, ChangeClassRunCancellationStatusCommand command)
        {
            classRuns.ForEach(classRun =>
            {
                classRun.CancellationStatus = command.CancellationStatus;
                classRun.ChangedBy = CurrentUserId;
                classRun.ChangedDate = Clock.Now;
            });
        }

        private void EnsureCanApproving(List<ClassRunAggregatedEntityModel> aggregatedClassRuns)
        {
            var hasAdminRightChecker = _readCourseRepository.GetHasAdminRightChecker(
                aggregatedClassRuns.Select(p => p.Course).DistinctBy(p => p.Id).ToList(), AccessControlContext);
            EnsureValidPermission(aggregatedClassRuns.TrueForAll(
                p => p.ClassRun.HasApprovalCancelClassRunPermission(p.Course, CurrentUserId, CurrentUserRoles, hasAdminRightChecker, permission => HasPermissionPrefix(permission))));

            EnsureBusinessLogicValid(Validation.FailFast(
                aggregatedClassRuns
                    .Select<ClassRunAggregatedEntityModel, Func<Validation>>(p => () => p.ClassRun.ValidateCanApprovalCancelClassRun(p.Course))
                    .ToArray()));
        }

        private void EnsureCanSubmitForApproving(List<ClassRunAggregatedEntityModel> aggregatedClassRuns)
        {
            var hasAdminRightChecker = _readCourseRepository.GetHasAdminRightChecker(
                aggregatedClassRuns.Select(p => p.Course).DistinctBy(p => p.Id).ToList(), AccessControlContext);
            EnsureValidPermission(aggregatedClassRuns.TrueForAll(
                p => p.ClassRun.HasCancelClassRunPermission(p.Course, CurrentUserId, CurrentUserRoles, hasAdminRightChecker, permission => HasPermissionPrefix(permission))));

            EnsureBusinessLogicValid(Validation.FailFast(
                aggregatedClassRuns
                    .Select<ClassRunAggregatedEntityModel, Func<Validation>>(p => () => p.ClassRun.ValidateCanCancelClassRun(p.Course))
                    .ToArray()));
        }
    }
}
