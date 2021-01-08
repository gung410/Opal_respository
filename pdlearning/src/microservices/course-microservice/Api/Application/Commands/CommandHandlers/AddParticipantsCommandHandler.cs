using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class AddParticipantsCommandHandler : BaseCommandHandler<AddParticipantsCommand>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly GetAggregatedClassRunSharedQuery _getAggregatedClassRunSharedQuery;
        private readonly RegistrationCudLogic _registrationCudLogic;
        private readonly BookWebinarMeetingLogic _bookWebinarMeetingLogic;

        public AddParticipantsCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            BookWebinarMeetingLogic bookWebinarMeetingLogic,
            IReadOnlyRepository<CourseUser> readUserRepository,
            GetAggregatedClassRunSharedQuery getAggregatedClassRunSharedQuery,
            RegistrationCudLogic registrationCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _readCourseRepository = readCourseRepository;
            _readUserRepository = readUserRepository;
            _getAggregatedClassRunSharedQuery = getAggregatedClassRunSharedQuery;
            _registrationCudLogic = registrationCudLogic;
            _bookWebinarMeetingLogic = bookWebinarMeetingLogic;
        }

        protected override async Task HandleAsync(AddParticipantsCommand command, CancellationToken cancellationToken)
        {
            var aggregatedClassRun = await _getAggregatedClassRunSharedQuery.ByClassRunId(command.ClassRunId);
            var courseHadAdminRightChecker = _readCourseRepository.GetHasAdminRightChecker(aggregatedClassRun.Course, AccessControlContext);

            EnsureValidPermission(
                aggregatedClassRun.Course.HasAddingParticipantsPermission(CurrentUserId, CurrentUserRoles, courseHadAdminRightChecker));

            EnsureBusinessLogicValid(aggregatedClassRun.ClassRun.ValidateCanAddParticipants(aggregatedClassRun.Course));

            var registrationIdsToReject = command.Items.Where(p => p.OtherInProgressRegistrationIds != null).SelectMany(x => x.OtherInProgressRegistrationIds).ToList();
            var registrationIdsToUpdate = command.Items.Where(p => p.RegistrationId != null).Select(a => a.RegistrationId.GetValueOrDefault()).ToList();
            var registrationsToUpdateDict = await _readRegistrationRepository.GetAll().Where(x => registrationIdsToUpdate.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

            var newRegistrationUserIds = command.Items.Where(p => p.RegistrationId == null).Select(a => a.UserId).ToList();
            var newRegistrationUsersDict = await _readUserRepository.GetAll()
                .Where(x => newRegistrationUserIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

            var newRegistrations = new List<Registration>();
            foreach (var item in command.Items)
            {
                var registrationStatus = item.Action switch
                {
                    AddParticipantsCommandRegistrationAction.SaveAddedByCAClassfull => RegistrationStatus.AddedByCAClassfull,
                    AddParticipantsCommandRegistrationAction.SaveAddedByCAConflict => RegistrationStatus.AddedByCAConflict,
                    AddParticipantsCommandRegistrationAction.SaveAddedByCASuccessfully => RegistrationStatus.ConfirmedByCA,
                    _ => RegistrationStatus.ConfirmedByCA
                };

                if (item.RegistrationId != null && registrationsToUpdateDict.ContainsKey(item.RegistrationId.Value))
                {
                    registrationsToUpdateDict[item.RegistrationId.Value].RegistrationType = RegistrationType.AddedByCA;
                    registrationsToUpdateDict[item.RegistrationId.Value].Status = registrationStatus;
                }
                else if (newRegistrationUsersDict.ContainsKey(item.UserId))
                {
                    var newRegistration = new Registration
                    {
                        Id = Guid.NewGuid(),
                        UserId = item.UserId,
                        ApprovingOfficer = newRegistrationUsersDict[item.UserId].PrimaryApprovingOfficerId,
                        AlternativeApprovingOfficer = newRegistrationUsersDict[item.UserId].AlternativeApprovingOfficerId,
                        ClassRunId = command.ClassRunId,
                        CourseId = aggregatedClassRun.Course.Id,
                        RegistrationDate = Clock.Now,
                        RegistrationType = RegistrationType.AddedByCA,
                        CreatedBy = CurrentUserIdOrDefault,
                        CreatedDate = Clock.Now,
                        Status = registrationStatus
                    };

                    newRegistrations.Add(newRegistration);
                }
            }

            var willBeUpdateRegistrations = registrationsToUpdateDict.Values.ToList();

            // Update list participants of Webinar when class started.
            var confirmedToBeParticipantRegistrations = newRegistrations
                .Concat(willBeUpdateRegistrations)
                .Where(p => p.IsParticipant)
                .ToList();
            await _bookWebinarMeetingLogic.UpdateMeeting(confirmedToBeParticipantRegistrations, cancellationToken);

            await CreateNewRegistration(newRegistrations, cancellationToken);
            await RejectRegistrations(registrationIdsToReject, cancellationToken);
            await UpdateRegistrations(willBeUpdateRegistrations, cancellationToken);
        }

        private async Task CreateNewRegistration(List<Registration> registrations, CancellationToken cancellationToken = default)
        {
            await _registrationCudLogic.InsertMany(registrations, cancellationToken);
        }

        private async Task RejectRegistrations(List<Guid> registrationIdsToReject, CancellationToken cancellationToken = default)
        {
            var registrationsToReject = await _readRegistrationRepository.GetAll()
                .Where(x => registrationIdsToReject.Contains(x.Id)).ToListAsync(cancellationToken);

            foreach (var registration in registrationsToReject)
            {
                registration.Status = RegistrationStatus.RejectedByCA;
            }

            await _registrationCudLogic.UpdateMany(registrationsToReject, cancellationToken);
        }

        private async Task UpdateRegistrations(List<Registration> registrationsToUpdate, CancellationToken cancellationToken = default)
        {
            await _registrationCudLogic.UpdateMany(registrationsToUpdate, cancellationToken);
        }
    }
}
