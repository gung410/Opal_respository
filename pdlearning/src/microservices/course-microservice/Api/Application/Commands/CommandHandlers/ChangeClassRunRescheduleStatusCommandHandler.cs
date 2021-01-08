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
using Microservice.Course.Application.Events.WebinarEvents.WebinarMeetingEvent;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Application.SharedQueries.GetHaveRegistrationsClassRuns;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Infrastructure;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;
using Thunder.Service.Authentication;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ChangeClassRunRescheduleStatusCommandHandler : BaseCommandHandler<ChangeClassRunRescheduleStatusCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly SendClassRunNotificationLogic _sendClassRunNotificationLogic;
        private readonly GetHaveRegistrationsClassRunsSharedQuery _getHaveRegistrationsClassRunsSharedQuery;
        private readonly GetAggregatedClassRunSharedQuery _getAggregatedClassRunSharedQuery;
        private readonly GetAggregatedSessionSharedQuery _getAggregatedSessionSharedQuery;
        private readonly GetBlockoutDateDependenciesSharedQuery _getBlockoutDateDependenciesSharedQuery;
        private readonly BookWebinarMeetingLogic _bookWebinarMeetingLogic;
        private readonly ClassRunCudLogic _classRunCudLogic;
        private readonly SessionCudLogic _sessionCudLogic;
        private readonly CourseCudLogic _courseCudLogic;
        private readonly RegistrationCudLogic _registrationCudLogic;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly SetRegistrationsExpiredLogic _setRegistrationsExpiredLogic;

        public ChangeClassRunRescheduleStatusCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<Session> readSessionRepository,
            IUserContext userContext,
            SendClassRunNotificationLogic sendClassRunNotificationLogic,
            IAccessControlContext<CourseUser> accessControlContext,
            GetHaveRegistrationsClassRunsSharedQuery getHaveRegistrationsClassRunsSharedQuery,
            ClassRunCudLogic classRunCudLogic,
            SessionCudLogic sessionCudLogic,
            CourseCudLogic courseCudLogic,
            RegistrationCudLogic registrationCudLogic,
            WebAppLinkBuilder webAppLinkBuilder,
            GetAggregatedClassRunSharedQuery getAggregatedClassRunSharedQuery,
            GetAggregatedSessionSharedQuery getAggregatedSessionSharedQuery,
            GetBlockoutDateDependenciesSharedQuery getBlockoutDateDependenciesSharedQuery,
            SetRegistrationsExpiredLogic setRegistrationsExpiredLogic,
            BookWebinarMeetingLogic bookWebinarMeetingLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _readSessionRepository = readSessionRepository;
            _sendClassRunNotificationLogic = sendClassRunNotificationLogic;
            _getHaveRegistrationsClassRunsSharedQuery = getHaveRegistrationsClassRunsSharedQuery;
            _classRunCudLogic = classRunCudLogic;
            _sessionCudLogic = sessionCudLogic;
            _courseCudLogic = courseCudLogic;
            _registrationCudLogic = registrationCudLogic;
            _webAppLinkBuilder = webAppLinkBuilder;
            _getAggregatedClassRunSharedQuery = getAggregatedClassRunSharedQuery;
            _getAggregatedSessionSharedQuery = getAggregatedSessionSharedQuery;
            _getBlockoutDateDependenciesSharedQuery = getBlockoutDateDependenciesSharedQuery;
            _bookWebinarMeetingLogic = bookWebinarMeetingLogic;
            _setRegistrationsExpiredLogic = setRegistrationsExpiredLogic;
        }

        protected override async Task HandleAsync(ChangeClassRunRescheduleStatusCommand command, CancellationToken cancellationToken)
        {
            var aggregatedClassRuns = await _getAggregatedClassRunSharedQuery.ByClassRunIds(command.Ids, true, cancellationToken);
            var classRuns = aggregatedClassRuns.SelectList(p => p.ClassRun);

            switch (command.RescheduleStatus)
            {
                case ClassRunRescheduleStatus.PendingApproval:
                    EnsureCanSubmitForApproving(aggregatedClassRuns);

                    await ChangeStatusForPendingApproval(classRuns, command, cancellationToken);

                    break;

                case ClassRunRescheduleStatus.Approved:
                    EnsureCanApproving(aggregatedClassRuns);

                    await ChangeStatusForApproved(aggregatedClassRuns, command, cancellationToken);

                    break;
                case ClassRunRescheduleStatus.Rejected:
                    EnsureCanApproving(aggregatedClassRuns);

                    ChangeStatusForRejected(classRuns, command);

                    break;
            }

            await _classRunCudLogic.UpdateMany(aggregatedClassRuns, cancellationToken);

            if (command.RescheduleStatus == ClassRunRescheduleStatus.Approved)
            {
                await SendNotificationWhenApproveReschedulingClassRun(classRuns, cancellationToken);
            }
        }

        private async Task SendNotificationWhenApproveReschedulingClassRun(
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
                (classRun, course) => new ApprovedRescheduleClassRunNotifyLearnerEvent(
                    (CurrentUserId == course.FirstAdministratorId || CurrentUserId == course.SecondAdministratorId) && CurrentUserId.HasValue ? CurrentUserIdOrDefault : course.FirstAdministratorId.GetValueOrDefault(),
                    new ApprovedRescheduleClassRunNotifyLearnerPayload
                    {
                        CourseName = course.CourseName,
                        CourseCode = course.CourseCode,
                        ClassTitle = classRun.ClassTitle,
                        RevisedStartDate = TimeHelper.ConvertTimeFromUtc(classRun.StartDateTime.GetValueOrDefault()).ToString(DateTimeFormatConstant.OnlyDate),
                        RevisedEndDate = TimeHelper.ConvertTimeFromUtc(classRun.EndDateTime.GetValueOrDefault()).ToString(DateTimeFormatConstant.OnlyDate),
                        ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(course.Id),
                        ObjectType = TodoEventPayloadObjectTypesConstant.Course,
                        ObjectId = course.Id
                    },
                    classRunIdToRegistrationsDic[classRun.Id].Select(x => x.UserId).ToList()),
                cancellationToken);
        }

        private async Task ChangeStatusForPendingApproval(List<ClassRun> classRuns, ChangeClassRunRescheduleStatusCommand command, CancellationToken cancellationToken)
        {
            classRuns.ForEach(classRun =>
            {
                classRun.RescheduleStatus = command.RescheduleStatus;
                classRun.RescheduleStartDateTime = command.StartDateTime;
                classRun.RescheduleEndDateTime = command.EndDateTime;
                classRun.ChangedBy = CurrentUserId;
                classRun.ChangedDate = Clock.Now;
            });

            var aggregatedSessions = await _getAggregatedSessionSharedQuery.ByQuery(_readSessionRepository.GetAll().Where(x => command.Ids.Contains(x.ClassRunId)), cancellationToken);
            var sessionRescheduleDic = command.RescheduleSessions.ToDictionary(x => x.Id, x => x);
            foreach (var aggregatedSession in aggregatedSessions)
            {
                var session = aggregatedSession.Session;
                var sessionReschedule = EnsureEntityFound(sessionRescheduleDic.GetValueOrDefault(session.Id, null));

                EnsureBusinessLogicValid(
                    await ValidateCanRescheduleSession(sessionReschedule.StartDateTime, aggregatedSession.Course.ServiceSchemeIds.ToList(), cancellationToken));

                session.RescheduleStartDateTime = sessionReschedule.StartDateTime;
                session.RescheduleEndDateTime = sessionReschedule.EndDateTime;
                session.ChangedBy = CurrentUserId;
                session.ChangedDate = Clock.Now;
            }

            await _sessionCudLogic.UpdateMany(aggregatedSessions, cancellationToken);
        }

        private async Task ChangeStatusForApproved(List<ClassRunAggregatedEntityModel> aggregatedClassRuns, ChangeClassRunRescheduleStatusCommand command, CancellationToken cancellationToken)
        {
            EnsureBusinessLogicValid(aggregatedClassRuns, p => p.ClassRun.ValidateCanReschedule(p.Course));

            aggregatedClassRuns.ForEach(aggregatedClassRun =>
            {
                var classRun = aggregatedClassRun.ClassRun;
                classRun.ChangedBy = CurrentUserId;
                classRun.ChangedDate = Clock.Now;
                classRun.StartDateTime = classRun.RescheduleStartDateTime;
                classRun.EndDateTime = classRun.RescheduleEndDateTime;
                classRun.PlanningStartTime = classRun.RescheduleStartDateTime;
                classRun.PlanningEndTime = classRun.RescheduleEndDateTime;
                classRun.RescheduleStatus = command.RescheduleStatus;

                var course = aggregatedClassRun.Course;
                course.ExpiredDate = classRun.EndDateTime > course.ExpiredDate ? classRun.EndDateTime : course.ExpiredDate;
                course.PlanningArchiveDate = course.ExpiredDate > course.PlanningArchiveDate ? course.ExpiredDate : course.PlanningArchiveDate;
            });
            await _courseCudLogic.UpdateMany(aggregatedClassRuns.SelectList(p => p.Course), cancellationToken);

            var aggregatedSessions = await _getAggregatedSessionSharedQuery.ByQuery(_readSessionRepository.GetAll().Where(x => command.Ids.Contains(x.ClassRunId)), cancellationToken);
            var sessions = aggregatedSessions.SelectList(p => p.Session);
            sessions.ForEach(session =>
            {
                session.ChangedBy = CurrentUserId;
                session.ChangedDate = Clock.Now;
                session.StartDateTime = session.RescheduleStartDateTime;
                session.EndDateTime = session.RescheduleEndDateTime;
            });
            await _sessionCudLogic.UpdateMany(aggregatedSessions, cancellationToken);
            await RescheduleWebinarMeetings(sessions, cancellationToken);

            var registrations = await _setRegistrationsExpiredLogic.Execute(aggregatedClassRuns.Select(p => p.ClassRun).ToList(), CurrentUserIdOrDefault, false, cancellationToken);
            await _registrationCudLogic.UpdateMany(registrations, cancellationToken);
        }

        private void ChangeStatusForRejected(List<ClassRun> classRuns, ChangeClassRunRescheduleStatusCommand command)
        {
            classRuns.ForEach(classRun =>
            {
                classRun.RescheduleStatus = command.RescheduleStatus;
                classRun.ChangedBy = CurrentUserId;
                classRun.ChangedDate = Clock.Now;
            });
        }

        private async Task RescheduleWebinarMeetings(List<Session> sessions, CancellationToken cancellationToken)
        {
            var notEndedSessions = sessions.Where(p => p.EndDateTime.HasValue && p.EndDateTime.Value > Clock.Now).ToList();
            await _bookWebinarMeetingLogic.BookMeeting(notEndedSessions, WebinarMeetingAction.Update, cancellationToken);
        }

        private async Task<Validation<DateTime>> ValidateCanRescheduleSession(DateTime sessionDate, List<string> serviceSchemes, CancellationToken cancellationToken)
        {
            var blockoutDateDependenciesModel = await _getBlockoutDateDependenciesSharedQuery.Execute(
                serviceSchemes,
                sessionDate,
                null,
                cancellationToken);

            var valid = UserRoles.IsSysAdministrator(CurrentUserRoles) || !blockoutDateDependenciesModel.MatchedBlockoutDates.Any();
            return Validation.ValidIf(sessionDate, valid, "Cannot reschedule this classrun since there is block-out date found at least at one of its rescheduling session dates");
        }

        private void EnsureCanSubmitForApproving(List<ClassRunAggregatedEntityModel> aggregatedClassRuns)
        {
            var hasAdminRightChecker = _readCourseRepository.GetHasAdminRightChecker(
                aggregatedClassRuns.Select(p => p.Course).DistinctBy(p => p.Id).ToList(), AccessControlContext);
            EnsureValidPermission(aggregatedClassRuns.TrueForAll(
                p => p.ClassRun.HasRescheduleClassRunPermission(p.Course, CurrentUserId, CurrentUserRoles, hasAdminRightChecker, permission => HasPermissionPrefix(permission))));

            EnsureBusinessLogicValid(Validation.FailFast(
                aggregatedClassRuns
                    .Select<ClassRunAggregatedEntityModel, Func<Validation>>(p => () => p.ClassRun.ValidateCanRescheduleClassRun(p.Course))
                    .ToArray()));
        }

        private void EnsureCanApproving(List<ClassRunAggregatedEntityModel> aggregatedClassRuns)
        {
            var hasAdminRightChecker = _readCourseRepository.GetHasAdminRightChecker(
                aggregatedClassRuns.Select(p => p.Course).DistinctBy(p => p.Id).ToList(), AccessControlContext);
            EnsureValidPermission(aggregatedClassRuns.TrueForAll(
                p => p.ClassRun.HasApprovalRescheduleClassRunPermission(p.Course, CurrentUserId, CurrentUserRoles, hasAdminRightChecker, permission => HasPermissionPrefix(permission))));

            EnsureBusinessLogicValid(Validation.FailFast(
                aggregatedClassRuns
                    .Select<ClassRunAggregatedEntityModel, Func<Validation>>(p => () => p.ClassRun.ValidateCanApprovalRescheduleClassRun(p.Course))
                    .ToArray()));
        }
    }
}
