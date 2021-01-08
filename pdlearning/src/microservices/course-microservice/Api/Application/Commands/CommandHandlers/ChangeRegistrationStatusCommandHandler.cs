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
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ChangeRegistrationStatusCommandHandler : BaseCommandHandler<ChangeRegistrationStatusCommand>
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<CourseCriteria> _readCourseCriteriaRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly SendRegistrationNotificationLogic _sendRegistrationNotificationLogic;
        private readonly CheckLearnerCourseCriteriaViolationLogic _checkLearnerCourseCriteriaViolationLogic;
        private readonly GetRemainingClassRunSlotSharedQuery _getRemainingClassRunSlotSharedQuery;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;
        private readonly RegistrationCudLogic _registrationCudLogic;
        private readonly ProcessForRegistrationConfirmByCALogic _processForRegistrationConfirmByCA;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly ProcessAutomateParticipantSelectionLogic _processAutomateParticipantSelectionLogic;
        private readonly ProcessRegistrationLogic _processRegistrationLogic;

        public ChangeRegistrationStatusCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<CourseCriteria> readCourseCriteriaRepository,
            IUserContext userContext,
            CheckLearnerCourseCriteriaViolationLogic checkLearnerCourseCriteriaViolationLogic,
            SendRegistrationNotificationLogic sendRegistrationNotificationLogic,
            IAccessControlContext<CourseUser> accessControlContext,
            GetRemainingClassRunSlotSharedQuery getRemainingClassRunSlotSharedQuery,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery,
            RegistrationCudLogic registrationCudLogic,
            ProcessForRegistrationConfirmByCALogic processForRegistrationConfirmByCA,
            ProcessAutomateParticipantSelectionLogic processAutomateParticipantSelectionLogic,
            ProcessRegistrationLogic processRegistrationLogic,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            WebAppLinkBuilder webAppLinkBuilder) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readClassRunRepository = readClassRunRepository;
            _readCourseCriteriaRepository = readCourseCriteriaRepository;
            _checkLearnerCourseCriteriaViolationLogic = checkLearnerCourseCriteriaViolationLogic;
            _sendRegistrationNotificationLogic = sendRegistrationNotificationLogic;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
            _registrationCudLogic = registrationCudLogic;
            _processForRegistrationConfirmByCA = processForRegistrationConfirmByCA;
            _readCourseRepository = readCourseRepository;
            _webAppLinkBuilder = webAppLinkBuilder;
            _processAutomateParticipantSelectionLogic = processAutomateParticipantSelectionLogic;
            _processRegistrationLogic = processRegistrationLogic;
            _getRemainingClassRunSlotSharedQuery = getRemainingClassRunSlotSharedQuery;
        }

        protected override async Task HandleAsync(
            ChangeRegistrationStatusCommand command,
            CancellationToken cancellationToken)
        {
            var aggregatedRegistrations = await _getAggregatedRegistrationSharedQuery.WithClassAndCourseByRegistrationIds(command.Ids, cancellationToken);

            EnsureEntitiesFound(aggregatedRegistrations);

            EnsureBusinessLogicValid(aggregatedRegistrations, p => p.Course.ValidateNotArchived().And(p.Registration.ValidateNotExpired()));

            var registrations = aggregatedRegistrations.Select(p => p.Registration).ToList();
            var hasRegistrationCourseFullRightChecker = _readCourseRepository.GetHasAdminRightChecker(
                aggregatedRegistrations.SelectList(p => p.Course),
                AccessControlContext);
            switch (command.Status)
            {
                case RegistrationStatus.Approved:
                    {
                        await ApproveRegistrations(aggregatedRegistrations, hasRegistrationCourseFullRightChecker, cancellationToken);
                        var approvedRegistrations = registrations.Where(x => x.Status == RegistrationStatus.Approved).ToList();
                        var waitlistRegistrations = registrations.Where(x => x.Status == RegistrationStatus.WaitlistConfirmed).ToList();
                        await _sendRegistrationNotificationLogic.SendToLearnerWhenOfficerApproved(approvedRegistrations, CurrentUserIdOrDefault, cancellationToken);
                        await _sendRegistrationNotificationLogic.SendToAdminWhenOfficerApproved(approvedRegistrations, cancellationToken);
                        await _sendRegistrationNotificationLogic.SendToAdminWhenWaitlistConfirmed(waitlistRegistrations);
                        break;
                    }

                case RegistrationStatus.Rejected:
                    {
                        RejectRegistrations(aggregatedRegistrations, hasRegistrationCourseFullRightChecker);
                        await _sendRegistrationNotificationLogic.SendToLearnerWhenOfficerRejected(registrations, CurrentUserIdOrDefault, cancellationToken);
                        break;
                    }

                case RegistrationStatus.ConfirmedByCA:
                    {
                        await ConfirmByCARegistrations(aggregatedRegistrations, hasRegistrationCourseFullRightChecker, cancellationToken);
                        await _processForRegistrationConfirmByCA.Execute(registrations, CurrentUserIdOrDefault, true, cancellationToken);

                        var waitlistRegistrations = registrations.Where(x => x.Status == RegistrationStatus.WaitlistConfirmed).ToList();
                        await _sendRegistrationNotificationLogic.SendToLearnerWhenWaitlistConfirmed(waitlistRegistrations, CurrentUserIdOrDefault, cancellationToken);
                        break;
                    }

                case RegistrationStatus.RejectedByCA:
                    {
                        RejectByCARegistrations(aggregatedRegistrations, hasRegistrationCourseFullRightChecker, command.Status);
                        await _sendRegistrationNotificationLogic.SendToLearnerWhenRejectedByCA(registrations, command.Comment, CurrentUserIdOrDefault);
                        break;
                    }

                case RegistrationStatus.WaitlistPendingApprovalByLearner:
                    {
                        MoveRegistrationsToWaitlist(aggregatedRegistrations, hasRegistrationCourseFullRightChecker, command.Status);
                        break;
                    }

                case RegistrationStatus.WaitlistRejected:
                case RegistrationStatus.WaitlistConfirmed:
                    {
                        LearnerApproveRejectWaitlist(aggregatedRegistrations, command.Status);
                        break;
                    }

                case RegistrationStatus.OfferPendingApprovalByLearner:
                    {
                        await OfferLearner(aggregatedRegistrations, hasRegistrationCourseFullRightChecker, cancellationToken);
                        break;
                    }

                case RegistrationStatus.OfferConfirmed:
                    {
                        LearnerConfirmRejectOffer(aggregatedRegistrations, command.Status);
                        await _processForRegistrationConfirmByCA.Execute(registrations, CurrentUserIdOrDefault, false, cancellationToken);
                        await _sendRegistrationNotificationLogic.SendToAdminWhenLearnerAcceptOffer(registrations, cancellationToken);
                        break;
                    }

                case RegistrationStatus.OfferRejected:
                    {
                        LearnerConfirmRejectOffer(aggregatedRegistrations, command.Status);
                        await _sendRegistrationNotificationLogic.SendToAdminWhenLearnerRejectOffer(registrations, cancellationToken);

                        // Process automate participant selection if classrun of each registration have available slots and course automate activated.
                        var selectedRegistrations = await _processAutomateParticipantSelectionLogic.ForNewAvailableSlot(registrations, CurrentUserIdOrDefault, cancellationToken);
                        await _registrationCudLogic.UpdateMany(selectedRegistrations, cancellationToken);
                        break;
                    }

                case RegistrationStatus.OfferExpired:
                    {
                        UpdateOfferExpired(aggregatedRegistrations, hasRegistrationCourseFullRightChecker, command.Status);

                        // Process automate participant selection if classrun of each registration have available slots and course automate activated when any registration was expired.
                        var selectedRegistrations = await _processAutomateParticipantSelectionLogic.ForNewAvailableSlot(registrations, CurrentUserIdOrDefault, cancellationToken);
                        await _registrationCudLogic.UpdateMany(selectedRegistrations, cancellationToken);
                        break;
                    }
            }

            await _registrationCudLogic.UpdateMany(registrations, cancellationToken);
        }

        private async Task OfferLearner(
            List<RegistrationAggregatedEntityModel> aggregatedRegistrations,
            Func<CourseEntity, bool> hasRegistrationCourseFullRightChecker,
            CancellationToken cancellationToken)
        {
            EnsureValidPermission(aggregatedRegistrations.TrueForAll(p =>
                p.Registration.HasManagePermission(p.Course, p.ClassRun, CurrentUserId, CurrentUserRoles, hasRegistrationCourseFullRightChecker, x => HasPermissionPrefix(x))));

            EnsureBusinessLogicValid(Validation.FailFast(
                aggregatedRegistrations.Select<RegistrationAggregatedEntityModel, Func<Validation>>(p => () => p.Registration.ValidateCanOfferLearnerManually()).ToArray()));

            var registrations = aggregatedRegistrations.SelectList(p => p.Registration);

            var classRunIds = registrations.Select(x => x.ClassRunId).ToList();
            var remainingSlotDict = await _getRemainingClassRunSlotSharedQuery.ByClassRunIds(classRunIds, cancellationToken);

            foreach (var registration in registrations)
            {
                if (remainingSlotDict.TryGetValue(registration.ClassRunId, out var remainSlot) && remainSlot > 0)
                {
                    registration.Status = RegistrationStatus.OfferPendingApprovalByLearner;
                    registration.AdministratedBy = CurrentUserId;
                    registration.AdministrationDate = Clock.Now;
                    registration.OfferSentDate = Clock.Now;
                    remainingSlotDict[registration.ClassRunId] -= 1;
                }
            }

            await _sendRegistrationNotificationLogic.ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new SendOfferNotifyLearnerEvent(
                    CurrentUserIdOrDefault,
                    new SendOfferNotifyLearnerPayload
                    {
                        CourseTitle = course.CourseName,
                        ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(course.Id),
                        ObjectType = TodoEventPayloadObjectTypesConstant.Course,
                        ObjectId = course.Id
                    },
                    new List<Guid> { registration.UserId }));
        }

        private void UpdateOfferExpired(
            List<RegistrationAggregatedEntityModel> aggregatedRegistrations,
            Func<CourseEntity, bool> hasRegistrationCourseFullRightChecker,
            RegistrationStatus status)
        {
            EnsureValidPermission(aggregatedRegistrations.TrueForAll(p =>
                p.Registration.HasManagePermission(p.Course, p.ClassRun, CurrentUserId, CurrentUserRoles, hasRegistrationCourseFullRightChecker, x => HasPermissionPrefix(x))));

            EnsureBusinessLogicValid(Validation.FailFast(
                aggregatedRegistrations.Select<RegistrationAggregatedEntityModel, Func<Validation>>(p => () => p.Registration.ValidateCanSetOfferExpired()).ToArray()));

            var registrations = aggregatedRegistrations.SelectList(p => p.Registration);

            registrations.ForEach(x =>
            {
                x.Status = status;
            });
        }

        private void LearnerConfirmRejectOffer(
            List<RegistrationAggregatedEntityModel> aggregatedRegistrations,
            RegistrationStatus status)
        {
            EnsureValidPermission(aggregatedRegistrations.TrueForAll(p =>
                p.Registration.HasApproveRejectOfferPermission(CurrentUserId, CurrentUserRoles)));

            EnsureBusinessLogicValid(Validation.FailFast(
                aggregatedRegistrations.Select<RegistrationAggregatedEntityModel, Func<Validation>>(p => () => p.Registration.ValidateCanApproveRejectOffer()).ToArray()));

            var registrations = aggregatedRegistrations.SelectList(p => p.Registration);

            registrations.ForEach(registration =>
            {
                registration.Status = status;
            });
        }

        private async Task ConfirmByCARegistrations(
            List<RegistrationAggregatedEntityModel> aggregatedRegistrations,
            Func<CourseEntity, bool> hasRegistrationCourseFullRightChecker,
            CancellationToken cancellationToken)
        {
            EnsureValidPermission(aggregatedRegistrations.TrueForAll(p =>
                p.Registration.HasManagePermission(p.Course, p.ClassRun, CurrentUserId, CurrentUserRoles, hasRegistrationCourseFullRightChecker, x => HasPermissionPrefix(x))));

            EnsureBusinessLogicValid(Validation.FailFast(
                aggregatedRegistrations.Select<RegistrationAggregatedEntityModel, Func<Validation>>(p => () => p.Registration.ValidateCanConfirmByCA()).ToArray()));

            var registrations = aggregatedRegistrations.SelectList(p => p.Registration);

            var classRunIds = registrations.Select(x => x.ClassRunId).Distinct().ToList();
            var remainingSlotDict = await _getRemainingClassRunSlotSharedQuery.ByClassRunIds(classRunIds, cancellationToken);

            registrations.ForEach(registration =>
            {
                registration.Status = _processRegistrationLogic.IsRegistrationClassFull(remainingSlotDict, registration.ClassRunId) ? RegistrationStatus.WaitlistConfirmed : RegistrationStatus.ConfirmedByCA;
                registration.AdministratedBy = CurrentUserId;
                registration.AdministrationDate = Clock.Now;
                remainingSlotDict[registration.ClassRunId]--;
            });
        }

        private void MoveRegistrationsToWaitlist(
            List<RegistrationAggregatedEntityModel> aggregatedRegistrations,
            Func<CourseEntity, bool> hasRegistrationCourseFullRightChecker,
            RegistrationStatus status)
        {
            EnsureValidPermission(aggregatedRegistrations.TrueForAll(p =>
                p.Registration.HasManagePermission(p.Course, p.ClassRun, CurrentUserId, CurrentUserRoles, hasRegistrationCourseFullRightChecker, x => HasPermissionPrefix(x))));

            EnsureBusinessLogicValid(Validation.FailFast(
                aggregatedRegistrations.Select<RegistrationAggregatedEntityModel, Func<Validation>>(p => () => p.Registration.ValidateCanMoveToWaitlistManually()).ToArray()));

            var registrations = aggregatedRegistrations.SelectList(p => p.Registration);

            registrations.ForEach(registration =>
            {
                registration.Status = status;
                registration.AdministratedBy = CurrentUserId;
                registration.AdministrationDate = Clock.Now;
            });
        }

        private void RejectByCARegistrations(
            List<RegistrationAggregatedEntityModel> aggregatedRegistrations,
            Func<CourseEntity, bool> hasRegistrationCourseFullRightChecker,
            RegistrationStatus status)
        {
            EnsureValidPermission(aggregatedRegistrations.TrueForAll(p =>
                p.Registration.HasManagePermission(p.Course, p.ClassRun, CurrentUserId, CurrentUserRoles, hasRegistrationCourseFullRightChecker, x => HasPermissionPrefix(x))));

            EnsureBusinessLogicValid(Validation.FailFast(
                aggregatedRegistrations.Select<RegistrationAggregatedEntityModel, Func<Validation>>(p => () => p.Registration.ValidateCanRejectByCA()).ToArray()));

            var registrations = aggregatedRegistrations.SelectList(p => p.Registration);

            registrations.ForEach(registration =>
            {
                registration.Status = status;
                registration.AdministratedBy = CurrentUserId;
                registration.AdministrationDate = Clock.Now;
            });
        }

        private void LearnerApproveRejectWaitlist(
            List<RegistrationAggregatedEntityModel> aggregatedRegistrations,
            RegistrationStatus status)
        {
            EnsureValidPermission(aggregatedRegistrations.TrueForAll(p =>
                p.Registration.HasApproveRejectWaitlistPermission(CurrentUserId, CurrentUserRoles)));

            EnsureBusinessLogicValid(Validation.FailFast(
                aggregatedRegistrations.Select<RegistrationAggregatedEntityModel, Func<Validation>>(p => () => p.Registration.ValidateCanApproveRejectWaitlist()).ToArray()));

            var registrations = aggregatedRegistrations.SelectList(p => p.Registration);

            registrations.ForEach(registration =>
            {
                registration.Status = status;
            });
        }

        private void RejectRegistrations(List<RegistrationAggregatedEntityModel> aggregatedRegistrations, Func<CourseEntity, bool> hasRegistrationCourseFullRightChecker)
        {
            EnsureValidPermission(aggregatedRegistrations.TrueForAll(p =>
                p.Registration.HasManagePermission(p.Course, p.ClassRun, CurrentUserId, CurrentUserRoles, hasRegistrationCourseFullRightChecker, x => HasPermissionPrefix(x))));

            EnsureBusinessLogicValid(
                Validation.FailFast(aggregatedRegistrations
                    .Select<RegistrationAggregatedEntityModel, Func<Validation>>(p => () => p.Registration.ValidateCanBeApproval())
                    .ToArray()));

            foreach (var aggregatedRegistration in aggregatedRegistrations)
            {
                aggregatedRegistration.Registration.Status = RegistrationStatus.Rejected;
                aggregatedRegistration.Registration.ApprovingDate = Clock.Now;
                aggregatedRegistration.Registration.ChangedBy = CurrentUserId;
            }
        }

        private async Task ApproveRegistrations(
            List<RegistrationAggregatedEntityModel> aggregatedRegistrations,
            Func<CourseEntity, bool> hasRegistrationCourseFullRightChecker,
            CancellationToken cancellationToken)
        {
            EnsureValidPermission(aggregatedRegistrations.TrueForAll(p =>
                p.Registration.HasManagePermission(p.Course, p.ClassRun, CurrentUserId, CurrentUserRoles, hasRegistrationCourseFullRightChecker, x => HasPermissionPrefix(x))));

            EnsureBusinessLogicValid(Validation.FailFast(
                aggregatedRegistrations.Select<RegistrationAggregatedEntityModel, Func<Validation>>(p => () => p.Registration.ValidateCanBeApproval()).ToArray()));

            var classRunIds = aggregatedRegistrations.Select(x => x.Registration.ClassRunId).Distinct().ToList();
            var classRunCriteriaActivatedDict = await _readClassRunRepository.GetAll().Where(p => classRunIds.Contains(p.Id))
                .Select(x => new { x.Id, x.CourseCriteriaActivated }).ToDictionaryAsync(p => p.Id, p => p.CourseCriteriaActivated, cancellationToken);

            var courseIds = aggregatedRegistrations.Select(x => x.Registration.CourseId).Distinct().ToList();
            var courseCriteriaDict = await _readCourseCriteriaRepository.GetAll().Where(p => courseIds.Contains(p.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);

            for (int i = 0; i < aggregatedRegistrations.Count; i++)
            {
                var registration = aggregatedRegistrations[i].Registration;
                registration.Status = RegistrationStatus.Approved;

                var hasCheckCourseCriteria = classRunCriteriaActivatedDict.ContainsKey(registration.ClassRunId) &&
                                             classRunCriteriaActivatedDict[registration.ClassRunId] &&
                                             courseCriteriaDict.ContainsKey(registration.CourseId);
                if (hasCheckCourseCriteria)
                {
                    var onMemoryProcessedRegistrations = aggregatedRegistrations.Take(i).ToList();
                    var courseCriteriaLearnerViolation = await _checkLearnerCourseCriteriaViolationLogic.Execute(
                        registration.CourseId,
                        registration.ClassRunId,
                        registration.UserId,
                        courseCriteriaDict[registration.CourseId],
                        onMemoryProcessedRegistrations.SelectList(p => p.Registration),
                        cancellationToken);

                    registration.CourseCriteriaViolation = courseCriteriaLearnerViolation ?? registration.CourseCriteriaViolation;

                    if (courseCriteriaLearnerViolation != null && courseCriteriaLearnerViolation.IsViolated)
                    {
                        registration.Status = RegistrationStatus.WaitlistConfirmed;
                    }
                }

                registration.ApprovingDate = Clock.Now;
                registration.ChangedBy = CurrentUserId;
            }

            // Process automate participant selection if classrun of each registration have available slots and course automate activated.
            var approvedRegistrations = aggregatedRegistrations.Where(x => x.Registration.Status == RegistrationStatus.Approved).ToList();
            var selectedRegistrations = await _processAutomateParticipantSelectionLogic.ForRegistrationApproved(
                approvedRegistrations.SelectList(p => p.Registration),
                CurrentUserIdOrDefault,
                cancellationToken);
            await _registrationCudLogic.UpdateMany(selectedRegistrations, cancellationToken);
        }
    }
}
