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
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ChangeClassRunCommandHandler : BaseCommandHandler<ChangeClassRunCommand>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly SendRegistrationNotificationLogic _sendRegistrationNotificationLogic;
        private readonly CheckBypassApprovalRegistrationLogic _checkBypassApprovalRegistrationLogic;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly ConfirmChangeClassRunRequestLogic _confirmChangeClassRunRequestLogic;
        private readonly RegistrationCudLogic _registrationCudLogic;

        public ChangeClassRunCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<CourseUser> readUserRepository,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery,
            IThunderCqrs thunderCqrs,
            IUserContext userContext,
            SendRegistrationNotificationLogic sendRegistrationNotificationLogic,
            CheckBypassApprovalRegistrationLogic checkBypassApprovalRegistrationLogic,
            RegistrationCudLogic registrationCudLogic,
            IAccessControlContext<CourseUser> accessControlContext,
            WebAppLinkBuilder webAppLinkBuilder,
            ConfirmChangeClassRunRequestLogic confirmChangeClassRunRequestLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _readClassRunRepository = readClassRunRepository;
            _readCourseRepository = readCourseRepository;
            _readUserRepository = readUserRepository;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
            _thunderCqrs = thunderCqrs;
            _sendRegistrationNotificationLogic = sendRegistrationNotificationLogic;
            _checkBypassApprovalRegistrationLogic = checkBypassApprovalRegistrationLogic;
            _webAppLinkBuilder = webAppLinkBuilder;
            _confirmChangeClassRunRequestLogic = confirmChangeClassRunRequestLogic;
            _registrationCudLogic = registrationCudLogic;
        }

        protected override async Task HandleAsync(ChangeClassRunCommand command, CancellationToken cancellationToken)
        {
            var aggregatedRegistrations = await _getAggregatedRegistrationSharedQuery.WithClassAndCourseByRegistrationIds(command.RegistrationIds, cancellationToken);
            var hasAdminRightChecker = _readCourseRepository.GetHasAdminRightChecker(aggregatedRegistrations.Select(p => p.Course).DistinctBy(p => p.Id).ToList(), AccessControlContext);

            EnsureValidPermission(
                aggregatedRegistrations,
                p => p.Registration.HasChangeClassRunPermission(p.Course, CurrentUserId, CurrentUserRoles, hasAdminRightChecker));

            await EnsureChangeClassRunRequestValid(command, aggregatedRegistrations, cancellationToken);

            var registrationBypassDict = await _checkBypassApprovalRegistrationLogic.Execute(aggregatedRegistrations.Select(x => x.Registration.Id).ToList(), cancellationToken);
            var changeClassRunRequestAggregatedRegistrations = aggregatedRegistrations
                .Select(p =>
                {
                    p.Registration.ClassRunChangeStatus = registrationBypassDict.GetValueOrDefault(p.Registration.Id) ? ClassRunChangeStatus.Approved : ClassRunChangeStatus.PendingConfirmation;
                    p.Registration.ClassRunChangeRequestedDate = Clock.Now;
                    p.Registration.ClassRunChangeId = command.ClassRunChangeId;
                    p.Registration.ChangedDate = Clock.Now;
                    p.Registration.ChangedBy = CurrentUserId;
                    return p;
                })
                .ToList();

            await _registrationCudLogic.UpdateMany(changeClassRunRequestAggregatedRegistrations.Select(p => p.Registration).ToList(), cancellationToken);

            var haveAdministrationPermissionRegistrations = changeClassRunRequestAggregatedRegistrations
                .Where(registration => registration.Course.HasAdministrationPermission(CurrentUserId, CurrentUserRoles, hasAdminRightChecker))
                .Select(p => p.Registration)
                .ToList();
            var registrationsSubmittedByLearners = changeClassRunRequestAggregatedRegistrations
                .Where(registration => !registration.Course.HasAdministrationPermission(CurrentUserId, CurrentUserRoles, hasAdminRightChecker))
                .Select(p => p.Registration)
                .ToList();

            if (haveAdministrationPermissionRegistrations.Any())
            {
                await AutoConfirmChangeRequestManuallyByCA(haveAdministrationPermissionRegistrations, cancellationToken);
                await SendChangeClassNotification(haveAdministrationPermissionRegistrations, cancellationToken);
            }

            if (registrationsSubmittedByLearners.Any())
            {
                await SendChangeClassNotificationByLearner(registrationsSubmittedByLearners);
            }
        }

        private async Task EnsureChangeClassRunRequestValid(
            ChangeClassRunCommand command,
            List<RegistrationAggregatedEntityModel> aggregatedRegistrations,
            CancellationToken cancellationToken)
        {
            EnsureBusinessLogicValid(aggregatedRegistrations, p => p.Registration.ValidateCanChangeClassRun(p.Course));

            var registrationUserIds = aggregatedRegistrations.Select(p => p.Registration.UserId).Distinct();
            var userHaveNoInProgressRegistrationInTargetChangeClassRun = !(await _readRegistrationRepository.GetAll()
                .Where(x => x.ClassRunId == command.ClassRunChangeId && registrationUserIds.Contains(x.UserId))
                .Where(Registration.InProgressExpr())
                .AnyAsync(cancellationToken));
            EnsureBusinessLogicValid(Validation.ValidIf(
                userHaveNoInProgressRegistrationInTargetChangeClassRun,
                "You can not change to classrun which already have your in progress registration"));

            var registrationClassRunIds = aggregatedRegistrations.Select(p => p.Registration.ClassRunId).Distinct();
            var registrationOrChangeToTargetClassRuns = await _readClassRunRepository.GetAllListAsync(x =>
                registrationClassRunIds.Contains(x.Id) || x.Id == command.ClassRunChangeId);
            EnsureBusinessLogicValid(Validation.ValidIf(
                registrationOrChangeToTargetClassRuns.All(p => p.PublishedAndNotStarted()),
                "The class run of the learner(s) or target change classrun must be not started"));
        }

        private async Task SendChangeClassNotification(
            List<Registration> haveAdministrationPermissionRegistrations,
            CancellationToken cancellationToken)
        {
            var courseIds = haveAdministrationPermissionRegistrations.Select(x => x.CourseId);
            var courseDict = await _readCourseRepository.GetDictionaryByIdsAsync(courseIds);
            var user = CurrentUserId.HasValue ? await _readUserRepository.GetAsync(CurrentUserIdOrDefault) : null;

            await _thunderCqrs.SendEvents(
                haveAdministrationPermissionRegistrations.Select(x =>
                    new ManualChangedClassLearnerNotifyLearnerEvent(
                        CurrentUserIdOrDefault,
                        new ManualChangedClassLearnerNotifyLearnerPayload
                        {
                            CAName = user != null ? user.FullName() : string.Empty,
                            CAEmail = user != null ? user.Email : string.Empty,
                            CourseTitle = courseDict.ContainsKey(x.CourseId) ? courseDict[x.CourseId].CourseName : string.Empty,
                            ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(x.CourseId),
                            ObjectType = TodoEventPayloadObjectTypesConstant.Course,
                            ObjectId = x.CourseId
                        },
                        new List<Guid> { x.UserId })),
                cancellationToken);
        }

        private async Task SendChangeClassNotificationByLearner(
            IEnumerable<Registration> registrations)
        {
            await _sendRegistrationNotificationLogic.ByRegistrations(
               registrations.ToList(),
               (registration, course, classRun, user) => new ChangeClassSubmittedByLearnerNotifyApproverEvent(
                   registration.Id,
                   new ChangeClassSubmittedByLearnerNotifyApproverPayload
                   {
                       LearnerName = user != null ? user.FullName() : string.Empty,
                       CourseTitle = course.CourseName,
                       ActionUrl = _webAppLinkBuilder.GetClassChangeRequestLinkForCAMModule()
                   },
                   user?.ApprovingOfficerIds));
        }

        private async Task AutoConfirmChangeRequestManuallyByCA(
            List<Registration> haveAdministrationPermissionRegistrations,
            CancellationToken cancellationToken)
        {
            await _confirmChangeClassRunRequestLogic.Execute(haveAdministrationPermissionRegistrations, CurrentUserIdOrDefault, cancellationToken, true);
        }
    }
}
