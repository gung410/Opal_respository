using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.BusinessLogics
{
    public class ConfirmChangeClassRunRequestLogic : BaseBusinessLogic
    {
        private readonly GetRemainingClassRunSlotSharedQuery _getRemainingClassRunSlotSharedQuery;
        private readonly RegistrationCudLogic _registrationCudLogic;
        private readonly ProcessAutomateParticipantSelectionLogic _processAutomateParticipantSelectionLogic;

        public ConfirmChangeClassRunRequestLogic(
            RegistrationCudLogic registrationCudLogic,
            GetRemainingClassRunSlotSharedQuery getRemainingClassRunSlotSharedQuery,
            ProcessAutomateParticipantSelectionLogic processAutomateParticipantSelectionLogic,
            IUserContext userContext) : base(userContext)
        {
            _getRemainingClassRunSlotSharedQuery = getRemainingClassRunSlotSharedQuery;
            _registrationCudLogic = registrationCudLogic;
            _processAutomateParticipantSelectionLogic = processAutomateParticipantSelectionLogic;
        }

        public async Task Execute(
            List<Registration> registrations,
            Guid currentUserId,
            CancellationToken cancellationToken,
            bool autoConfirmNewChangedClassrunRegistration = false)
        {
            var classRunChangeIds = registrations.Where(x => x.ClassRunChangeId.HasValue).Select(x => x.ClassRunChangeId.Value).Distinct().ToList();

            var remainingSlotDict = await _getRemainingClassRunSlotSharedQuery.ByClassRunIds(classRunChangeIds, cancellationToken);

            var confirmedClassRunChangeStatusRegistrations = registrations
                .Select(registration =>
                {
                    registration.ClassRunChangeStatus = ClassRunChangeStatus.ConfirmedByCA;
                    registration.AdministratedBy = currentUserId;
                    registration.AdministrationDate = Clock.Now;
                    registration.ChangedBy = currentUserId;
                    return registration;
                })
                .ToList();
            var forNewChangedClassRunRegistrations = confirmedClassRunChangeStatusRegistrations
                .Select(registration => CreateRegistrationForNewChangedClassRun(registration, remainingSlotDict, autoConfirmNewChangedClassrunRegistration))
                .ToList();

            // Process automate participant selection if classrun of each registration have available slots and course automate activated
            // when any registration in the old classrun was changed into the new classrun.
            var selectedRegistrations = await _processAutomateParticipantSelectionLogic.ForNewAvailableSlot(confirmedClassRunChangeStatusRegistrations, currentUserId, cancellationToken);
            await _registrationCudLogic.UpdateMany(selectedRegistrations, cancellationToken);

            await _registrationCudLogic.InsertMany(forNewChangedClassRunRegistrations, cancellationToken);
            await _registrationCudLogic.UpdateMany(confirmedClassRunChangeStatusRegistrations, cancellationToken);
        }

        private Registration CreateRegistrationForNewChangedClassRun(
            Registration oldRegistration,
            Dictionary<Guid, int> remainingSlotDict,
            bool autoConfirmNewChangedClassrunRegistration)
        {
            Registration newRegistration = new Registration
            {
                Id = Guid.NewGuid(),
                UserId = oldRegistration.UserId,
                CourseId = oldRegistration.CourseId,
                ClassRunId = oldRegistration.ClassRunChangeId.GetValueOrDefault(),
                RegistrationDate = Clock.Now,
                ApprovingOfficer = oldRegistration.ApprovingOfficer,
                AlternativeApprovingOfficer = oldRegistration.AlternativeApprovingOfficer,
                ApprovingDate = Clock.Now,
                RegistrationType = oldRegistration.RegistrationType,
                CreatedBy = oldRegistration.UserId,
                ExternalId = null,
                IsDeleted = false,
                Status = autoConfirmNewChangedClassrunRegistration
                    ? RegistrationStatus.ConfirmedByCA
                    : RegistrationStatus.Approved
            };
            remainingSlotDict[newRegistration.ClassRunId] -= 1;

            return newRegistration;
        }
    }
}
