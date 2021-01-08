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
    public class ChangeRegistrationWithdrawStatusCommandHandler : BaseCommandHandler<ChangeRegistrationWithdrawStatusCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly SendRegistrationWithdrawalNotificationLogic _sendRegistrationWithdrawalNotificationLogic;
        private readonly SendRegistrationNotificationLogic _sendRegistrationNotificationLogic;
        private readonly CheckBypassApprovalRegistrationLogic _checkBypassApprovalRegistrationLogic;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly RegistrationCudLogic _registrationCudLogic;
        private readonly ProcessAutomateParticipantSelectionLogic _processAutomateParticipantSelectionLogic;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;

        public ChangeRegistrationWithdrawStatusCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUserContext userContext,
            SendRegistrationWithdrawalNotificationLogic sendRegistrationWithdrawalNotificationLogic,
            SendRegistrationNotificationLogic sendRegistrationNotificationLogic,
            CheckBypassApprovalRegistrationLogic checkBypassApprovalRegistrationLogic,
            IAccessControlContext<CourseUser> accessControlContext,
            WebAppLinkBuilder webAppLinkBuilder,
            ProcessAutomateParticipantSelectionLogic processAutomateParticipantSelectionLogic,
            RegistrationCudLogic registrationCudLogic,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _sendRegistrationWithdrawalNotificationLogic = sendRegistrationWithdrawalNotificationLogic;
            _sendRegistrationNotificationLogic = sendRegistrationNotificationLogic;
            _checkBypassApprovalRegistrationLogic = checkBypassApprovalRegistrationLogic;
            _webAppLinkBuilder = webAppLinkBuilder;
            _processAutomateParticipantSelectionLogic = processAutomateParticipantSelectionLogic;
            _registrationCudLogic = registrationCudLogic;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
        }

        protected override async Task HandleAsync(ChangeRegistrationWithdrawStatusCommand command, CancellationToken cancellationToken)
        {
            var aggregatedRegistrations = await _getAggregatedRegistrationSharedQuery.WithClassAndCourseByRegistrationIds(command.Ids, cancellationToken);

            EnsureEntitiesFound(aggregatedRegistrations);

            var registrations = aggregatedRegistrations.SelectList(p => p.Registration);
            switch (command.WithdrawalStatus)
            {
                case WithdrawalStatus.PendingConfirmation:
                    {
                        await CreateWithdrawRequest(aggregatedRegistrations, cancellationToken);
                        break;
                    }

                case WithdrawalStatus.Approved:
                case WithdrawalStatus.Rejected:
                    {
                        EnsureHasManageRegistrationsPermission(aggregatedRegistrations);
                        registrations.ForEach(registration =>
                        {
                            registration.WithdrawalStatus = command.WithdrawalStatus;
                            registration.ChangedBy = CurrentUserId;
                        });

                        switch (command.WithdrawalStatus)
                        {
                            case WithdrawalStatus.Approved:
                                await SendNotificationWhenLearnerWithdrawnClassrun(registrations);
                                break;
                            case WithdrawalStatus.Rejected:
                                await SendNotificationToLearnerWhenRejectedWithdrawalClassrunByApprover(registrations);
                                break;
                        }

                        break;
                    }

                case WithdrawalStatus.Withdrawn:
                case WithdrawalStatus.RejectedByCA:
                    {
                        await ApproveRejectWithdrawRequest(aggregatedRegistrations, command.WithdrawalStatus, command.IsManual, cancellationToken);
                        break;
                    }
            }

            // Process automate participant selection if classrun of each registration have available slots and course automate activated when any registration in classrun was withdrawn.
            if (command.WithdrawalStatus == WithdrawalStatus.Withdrawn)
            {
                var selectedRegistrations = await _processAutomateParticipantSelectionLogic.ForNewAvailableSlot(registrations, CurrentUserIdOrDefault, cancellationToken);
                registrations.AddRange(selectedRegistrations);
            }

            await _registrationCudLogic.UpdateMany(registrations, cancellationToken);
        }

        private List<Registration> EnsureHasManageRegistrationsPermission(List<RegistrationAggregatedEntityModel> aggregatedRegistrations)
        {
            var registrations = aggregatedRegistrations.SelectList(p => p.Registration);
            var hasRegistrationCourseFullRightChecker = _readCourseRepository.GetHasAdminRightChecker(
                aggregatedRegistrations.SelectList(p => p.Course),
                AccessControlContext);
            EnsureValidPermission(aggregatedRegistrations.TrueForAll(p =>
                p.Registration.HasManagePermission(
                    p.Course, p.ClassRun, CurrentUserId, CurrentUserRoles, hasRegistrationCourseFullRightChecker, x => HasPermissionPrefix(x))));
            return registrations;
        }

        private async Task SendNotificationWhenLearnerWithdrawnClassrun(
            List<Registration> registrations)
        {
            await _sendRegistrationNotificationLogic.ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new WithdrawClassrunNotifyAdministratorEvent(
                    registration.UserId,
                    new WithdrawClassrunNotifyAdministratorPayload
                    {
                        CourseTitle = course.CourseName,
                        CourseRunTitle = classRun.ClassTitle,
                        ActionUrl = _webAppLinkBuilder.GetClassRunDetailLinkForCAMModule(
                                        CAMTabConfigurationConstant.HasPendingRegistrationApprovalCourseTab,
                                        CAMTabConfigurationConstant.ClassRunsTab,
                                        CAMTabConfigurationConstant.HasPendingRegistrationApprovalClassrunTab,
                                        CourseDetailModeConstant.View,
                                        CAMTabConfigurationConstant.WithdrawalRequestsTab,
                                        ClassRunDetailModeConstant.View,
                                        course.Id,
                                        classRun.Id)
                    },
                    course.GetAdministratorIds()));
        }

        private async Task SendNotificationToLearnerWhenRejectedWithdrawalClassrunByApprover(
            List<Registration> registrations)
        {
            await _sendRegistrationNotificationLogic.ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new WithdrawalRejectedNotifyLearnerByApproverEvent(
                    CurrentUserIdOrDefault,
                    new WithdrawalRejectedNotifyLearnerByApproverPayload
                    {
                        CourseTitle = course.CourseName,
                        ClassrunTitle = classRun.ClassTitle,
                        ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(course.Id),
                        ObjectType = TodoEventPayloadObjectTypesConstant.Course,
                        ObjectId = course.Id
                    },
                    new List<Guid> { registration.UserId }));
        }

        private async Task SendNotificationToLearnerWhenRejectedWithdrawalClassrunByAdministrator(
            List<Registration> registrations)
        {
            await _sendRegistrationNotificationLogic.ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new WithdrawalRejectedNotifyLearnerByAdministratorEvent(
                    CurrentUserIdOrDefault,
                    new WithdrawalRejectedNotifyLearnerByAdministratorPayload
                    {
                        CourseTitle = course.CourseName,
                        ClassrunTitle = classRun.ClassTitle,
                        ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(course.Id),
                        ObjectType = TodoEventPayloadObjectTypesConstant.Course,
                        ObjectId = course.Id
                    },
                    new List<Guid> { registration.UserId }));
        }

        private async Task SendNotificationToLearnerWhenConfirmedWithdrawalClassrunByAdministrator(
            List<Registration> registrations)
        {
            await _sendRegistrationNotificationLogic.ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new WithdrawalConfirmedNotifyLearnerByAdministratorEvent(
                    CurrentUserIdOrDefault,
                    new WithdrawalConfirmedNotifyLearnerByAdministratorPayload
                    {
                        CourseTitle = course.CourseName,
                        ClassrunTitle = classRun.ClassTitle,
                        ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(course.Id),
                        ObjectType = TodoEventPayloadObjectTypesConstant.Course,
                        ObjectId = course.Id
                    },
                    new List<Guid> { registration.UserId }));
        }

        private async Task ApproveRejectWithdrawRequest(List<RegistrationAggregatedEntityModel> aggregatedRegistrations, WithdrawalStatus withdrawalStatus, bool isManual, CancellationToken cancellationToken)
        {
            EnsureHasManageRegistrationsPermission(aggregatedRegistrations);

            EnsureBusinessLogicValid(Validation.FailFast(
                aggregatedRegistrations.Select<RegistrationAggregatedEntityModel, Func<Validation>>(p => () => p.Registration.ValidateCanApproveRejectWithdrawRequest(p.Course)).ToArray()));

            if (withdrawalStatus == WithdrawalStatus.Withdrawn)
            {
                EnsureHasManageRegistrationsPermission(aggregatedRegistrations);
            }

            var registrations = aggregatedRegistrations.SelectList(p => p.Registration);

            registrations.ForEach(registration =>
            {
                registration.WithdrawalStatus = withdrawalStatus;
                registration.AdministratedBy = CurrentUserId;
                registration.AdministrationDate = Clock.Now;
                registration.ChangedBy = CurrentUserId;
                registration.WithdrawalRequestDate = isManual ? Clock.Now : registration.WithdrawalRequestDate;
            });

            if (isManual)
            {
                await _sendRegistrationWithdrawalNotificationLogic.Execute(registrations, CurrentUserIdOrDefault, cancellationToken);
            }

            if (withdrawalStatus == WithdrawalStatus.Withdrawn && !isManual)
            {
                await SendNotificationToLearnerWhenConfirmedWithdrawalClassrunByAdministrator(registrations);
            }

            if (withdrawalStatus == WithdrawalStatus.RejectedByCA && !isManual)
            {
                await SendNotificationToLearnerWhenRejectedWithdrawalClassrunByAdministrator(registrations);
            }
        }

        private async Task CreateWithdrawRequest(List<RegistrationAggregatedEntityModel> aggregatedRegistrations, CancellationToken cancellationToken)
        {
            EnsureValidPermission(aggregatedRegistrations.TrueForAll(p => p.Registration.HasCreateWithdrawRequestPermission(CurrentUserId, CurrentUserRoles)));

            EnsureBusinessLogicValid(Validation.FailFast(
                aggregatedRegistrations.Select<RegistrationAggregatedEntityModel, Func<Validation>>(p => () => p.Registration.ValidateCanCreateWithdrawRequest(p.Course, p.ClassRun)).ToArray()));

            var registrations = aggregatedRegistrations.SelectList(p => p.Registration);

            var registrationBypassDict = await _checkBypassApprovalRegistrationLogic.Execute(registrations.Select(x => x.Id).ToList(), cancellationToken);

            registrations.ForEach(registration =>
            {
                registration.WithdrawalStatus = registrationBypassDict.GetOrDefault(registration.Id) ? WithdrawalStatus.Approved : WithdrawalStatus.PendingConfirmation;
                registration.WithdrawalRequestDate = Clock.Now;
                registration.ChangedBy = CurrentUserId;
            });

            await SendNotificationWhenLearnerSubmitWithdrawClassrun(registrations);
        }

        private async Task SendNotificationWhenLearnerSubmitWithdrawClassrun(
           List<Registration> registrations)
        {
            await _sendRegistrationNotificationLogic.ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new SubmittedWithdrawClassrunNotifyApproverEvent(
                    registration.UserId,
                    new SubmittedWithdrawClassrunNotifyApproverPayload
                    {
                        CourseTitle = course.CourseName,
                        LearnerName = user.FullName(),
                        ActionUrl = _webAppLinkBuilder.GetClassWithdrawalLinkForCAMModule()
                    },
                    user.ApprovingOfficerIds));
        }
    }
}
