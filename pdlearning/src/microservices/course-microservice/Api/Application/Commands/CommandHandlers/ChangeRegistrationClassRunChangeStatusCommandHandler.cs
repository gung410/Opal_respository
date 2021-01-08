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
using Microservice.Course.Application.Events;
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
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ChangeRegistrationClassRunChangeStatusCommandHandler : BaseCommandHandler<ChangeRegistrationClassRunChangeStatusCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly ConfirmChangeClassRunRequestLogic _confirmChangeClassRunRequest;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly RegistrationCudLogic _registrationCudLogic;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;

        public ChangeRegistrationClassRunChangeStatusCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<CourseUser> readUserRepository,
            RegistrationCudLogic registrationCudLogic,
            IThunderCqrs thunderCqrs,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            ConfirmChangeClassRunRequestLogic confirmChangeClassRunRequest,
            WebAppLinkBuilder webAppLinkBuilder,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readUserRepository = readUserRepository;
            _readCourseRepository = readCourseRepository;
            _registrationCudLogic = registrationCudLogic;
            _thunderCqrs = thunderCqrs;
            _confirmChangeClassRunRequest = confirmChangeClassRunRequest;
            _webAppLinkBuilder = webAppLinkBuilder;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
        }

        protected override async Task HandleAsync(ChangeRegistrationClassRunChangeStatusCommand command, CancellationToken cancellationToken)
        {
            var aggregatedRegistrations = await _getAggregatedRegistrationSharedQuery.WithClassAndCourseByRegistrationIds(command.Ids, cancellationToken);

            EnsureEntitiesFound(aggregatedRegistrations);

            var hasRegistrationCourseFullRightChecker = _readCourseRepository.GetHasAdminRightChecker(
                aggregatedRegistrations.SelectList(p => p.Course),
                AccessControlContext);
            EnsureValidPermission(aggregatedRegistrations.TrueForAll(p =>
                p.Registration.HasManagePermission(p.Course, p.ClassRun, CurrentUserId, CurrentUserRoles, hasRegistrationCourseFullRightChecker, x => HasPermissionPrefix(x))));

            var registrations = aggregatedRegistrations.SelectList(p => p.Registration);
            switch (command.ClassRunChangeStatus)
            {
                case ClassRunChangeStatus.Approved:
                case ClassRunChangeStatus.Rejected:
                    {
                        await AOApproveRejectRequest(aggregatedRegistrations, command.ClassRunChangeStatus, cancellationToken);
                        break;
                    }

                case ClassRunChangeStatus.ConfirmedByCA:
                    {
                        await _confirmChangeClassRunRequest.Execute(registrations, CurrentUserIdOrDefault, cancellationToken);
                        break;
                    }

                case ClassRunChangeStatus.RejectedByCA:
                    {
                        await RejectChangeClassRunRequest(aggregatedRegistrations, command.ClassRunChangeStatus, cancellationToken);
                        break;
                    }
            }
        }

        private async Task AOApproveRejectRequest(List<RegistrationAggregatedEntityModel> aggregatedRegistrations, ClassRunChangeStatus newStatus, CancellationToken cancellationToken)
        {
            EnsureBusinessLogicValid(Validation.FailFast(
                aggregatedRegistrations.Select<RegistrationAggregatedEntityModel, Func<Validation>>(p => () => p.Registration.ValidateCanApproveRejectChangeClassRunRequest(p.Course)).ToArray()));

            var registrations = aggregatedRegistrations.SelectList(p => p.Registration);

            registrations.ForEach(registration =>
            {
                registration.ClassRunChangeStatus = newStatus;
                registration.ChangedBy = CurrentUserId;
            });

            await _registrationCudLogic.UpdateMany(registrations, cancellationToken);

            if (newStatus == ClassRunChangeStatus.Approved)
            {
                await SendChangeClassNotificationToAdministrators(registrations, cancellationToken);
            }
        }

        private async Task SendChangeClassNotificationToAdministrators(List<Registration> registrations, CancellationToken cancellationToken)
        {
            var courseIds = registrations.Select(x => x.CourseId);
            var courseDict = await _readCourseRepository.GetDictionaryByIdsAsync(courseIds);
            var learnerIds = registrations.Select(x => x.UserId);
            var learnerDict = await _readUserRepository.GetAll().Where(x => learnerIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x, cancellationToken);
            await _thunderCqrs.SendEvents(
                registrations.Select(x =>
                    new ChangeClassLearnerNotifyAdministratorEvent(
                        CurrentUserIdOrDefault,
                        new ChangeClassLearnerNotifyAdministratorPayload
                        {
                            CourseTitle = courseDict[x.CourseId].CourseName,
                            LearnerName = learnerDict[x.UserId].FullName(),
                            ActionUrl = _webAppLinkBuilder.GetClassRunDetailLinkForCAMModule(
                                CAMTabConfigurationConstant.HasPendingRegistrationApprovalCourseTab,
                                CAMTabConfigurationConstant.ClassRunsTab,
                                CAMTabConfigurationConstant.HasPendingRegistrationApprovalClassrunTab,
                                CourseDetailModeConstant.View,
                                CAMTabConfigurationConstant.ChangeClassRunRequestsTab,
                                ClassRunDetailModeConstant.View,
                                x.CourseId,
                                x.ClassRunId)
                        },
                        courseDict[x.CourseId].GetAdministratorIds())),
                cancellationToken);
        }

        private async Task RejectChangeClassRunRequest(List<RegistrationAggregatedEntityModel> registrations, ClassRunChangeStatus newStatus, CancellationToken cancellationToken)
        {
            registrations.ForEach(p =>
            {
                p.Registration.ClassRunChangeStatus = newStatus;
                p.Registration.AdministratedBy = CurrentUserId;
                p.Registration.AdministrationDate = Clock.Now;
            });

            await _registrationCudLogic.UpdateMany(registrations, cancellationToken);
        }
    }
}
