using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.BusinessLogics
{
    public class ProcessAutomateParticipantSelectionLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassrunRepository;
        private readonly GetRemainingClassRunSlotSharedQuery _getRemainingClassRunSlotSharedQuery;
        private readonly SendRegistrationNotificationLogic _sendRegistrationNotificationLogic;

        public ProcessAutomateParticipantSelectionLogic(
           IReadOnlyRepository<Registration> readRegistrationRepository,
           IReadOnlyRepository<ClassRun> readClassrunRepository,
           SendRegistrationNotificationLogic sendRegistrationNotificationLogic,
           GetRemainingClassRunSlotSharedQuery getRemainingClassRunSlotSharedQuery,
           IUserContext userContext) : base(userContext)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _readClassrunRepository = readClassrunRepository;
            _sendRegistrationNotificationLogic = sendRegistrationNotificationLogic;
            _getRemainingClassRunSlotSharedQuery = getRemainingClassRunSlotSharedQuery;
        }

        /// <summary>
        /// Process for rejected or expired offer registrations.
        /// </summary>
        /// <param name="willBeRemovedParticipants">Offer was rejected by learner or expired.</param>
        /// <param name="currentUserId">Current User.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Return list registration after processing.</returns>
        public async Task<List<Registration>> ForNewAvailableSlot(
            List<Registration> willBeRemovedParticipants,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var classrunIds = willBeRemovedParticipants.Select(x => x.ClassRunId).Distinct().ToList();

            // Filter class-runs which has CourseAutomateActivated is true.
            var automateActivatedClassrunIds = await _readClassrunRepository.GetAll()
                .Where(x => classrunIds.Contains(x.Id))
                .Where(x => x.CourseAutomateActivated)
                .Select(x => x.Id).ToListAsync(cancellationToken);

            return await ByClassRunIds(automateActivatedClassrunIds, willBeRemovedParticipants, currentUserId, cancellationToken);
        }

        /// <summary>
        /// Process for Registration Approved.
        /// </summary>
        /// <param name="approvedRegistrations">Participants are be approved.</param>
        /// <param name="currentUserId">Current User.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Return list registration after processing.</returns>
        public async Task<List<Registration>> ForRegistrationApproved(
            List<Registration> approvedRegistrations,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var classrunIds = approvedRegistrations.Select(x => x.ClassRunId).Distinct().ToList();

            // Filter class-runs which has CourseAutomateActivated is true.
            var automateActivatedClassrunIds = await _readClassrunRepository
                .GetAll()
                .Where(x => classrunIds.Contains(x.Id))
                .Where(x => x.CourseAutomateActivated)
                .Select(x => x.Id).ToListAsync(cancellationToken);

            return await ByClassRunIds(automateActivatedClassrunIds, null, currentUserId, cancellationToken);
        }

        /// <summary>
        /// Process for all input classrun.
        /// </summary>
        /// <param name="classrunIds">List Classrun.</param>
        /// <param name="willBeRemovedParticipants">Participants will be removed if participant is change class or withdraw or offer expired.</param>
        /// <param name="currentUserId">Current User.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Return list registration after processing.</returns>
        public async Task<List<Registration>> ByClassRunIds(
        List<Guid> classrunIds,
        List<Registration> willBeRemovedParticipants,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
        {
            // Get not started class-runs
            var notStartedClassRuns = await _readClassrunRepository
               .GetAll()
               .Where(x => classrunIds.Contains(x.Id))
               .Where(ClassRun.NotStartedExpr())
               .ToListAsync(cancellationToken);
            var notStartedClassRunIds = notStartedClassRuns.Select(p => p.Id).ToList();

            // Get remaining slot dictionary for class-runs
            var remainingSlotDict = willBeRemovedParticipants != null
                ? await _getRemainingClassRunSlotSharedQuery.ByClassRunIds(notStartedClassRunIds, willBeRemovedParticipants, cancellationToken)
                : await _getRemainingClassRunSlotSharedQuery.ByClassRunIds(notStartedClassRunIds, cancellationToken);

            // Waiting to be selected as participant registrations group by classrun.
            var classRunToRegistrationsDic = (await _readRegistrationRepository
                    .GetAll()
                    .Where(x => notStartedClassRunIds.Contains(x.ClassRunId))
                    .Where(Registration.WaitingForParticipantSelectionExpr())
                    .OrderBy(x => x.RegistrationDate)
                    .ToListAsync(cancellationToken))
                    .GroupBy(p => p.ClassRunId)
                    .ToDictionary(p => p.Key, p => p.ToList());

            // Filter class-runs which have available slots and have registrations waiting to be selected as participants.
            var slotAvailableNotStartedClassRuns = notStartedClassRuns.Where(p => remainingSlotDict[p.Id] > 0 && classRunToRegistrationsDic.ContainsKey(p.Id));

            // Select registrations to be participants/offer participants. Return selected registrations.
            var selectedRegistrations = slotAvailableNotStartedClassRuns
                .SelectMany(classrun =>
                {
                    var registrations = classRunToRegistrationsDic[classrun.Id];
                    return registrations
                        .Where(registration => registration.CanPassCourseCriteriaCheckForClassRun(classrun))
                        .Select(registration =>
                        {
                            // If there are available slots:
                            if (remainingSlotDict[classrun.Id] > 0)
                            {
                                // If registration is approved by AO, process it to be a participant.
                                if (registration.Status == RegistrationStatus.Approved)
                                {
                                    registration.Status = RegistrationStatus.ConfirmedByCA;
                                    registration.ApprovingDate = Clock.Now;
                                    registration.ChangedBy = currentUserId;
                                    remainingSlotDict[classrun.Id] -= 1;
                                    return new { registration, isChanged = true };
                                }

                                // If registration is in waitlist, send offer to learner.
                                else if (registration.Status == RegistrationStatus.WaitlistConfirmed)
                                {
                                    registration.Status = RegistrationStatus.OfferPendingApprovalByLearner;
                                    registration.AdministratedBy = currentUserId;
                                    registration.AdministrationDate = Clock.Now;
                                    registration.OfferSentDate = Clock.Now;
                                    remainingSlotDict[classrun.Id] -= 1;
                                    return new { registration, isChanged = true };
                                }
                            }
                            else
                            {
                                registration.Status = RegistrationStatus.WaitlistConfirmed;
                                return new { registration, isChanged = true };
                            }

                            return new { registration, isChanged = false };
                        })
                        .Where(p => p.isChanged)
                        .Select(p => p.registration);
                })
                .ToList();

            await _sendRegistrationNotificationLogic.SendToLearnerWhenOfferSent(selectedRegistrations.Where(p => p.Status == RegistrationStatus.OfferPendingApprovalByLearner).ToList(), currentUserId);
            await _sendRegistrationNotificationLogic.SendToLearnerWhenRegistrationConfirmed(selectedRegistrations.Where(p => p.Status == RegistrationStatus.ConfirmedByCA).ToList(), currentUserId);
            return selectedRegistrations;
        }
    }
}
