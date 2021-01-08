using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AssociatedEntities;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Events;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics
{
    public class ProcessForRegistrationConfirmByCALogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly SendRegistrationNotificationLogic _sendRegistrationNotificationLogic;
        private readonly BookWebinarMeetingLogic _bookWebinarMeetingLogic;
        private readonly RegistrationCudLogic _registrationCudLogic;

        public ProcessForRegistrationConfirmByCALogic(
            IReadOnlyRepository<Registration> readRegistrationRepository,
            SendRegistrationNotificationLogic sendRegistrationNotificationLogic,
            BookWebinarMeetingLogic bookWebinarMeetingLogic,
            IUserContext userContext,
            RegistrationCudLogic registrationCudLogic) : base(userContext)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _sendRegistrationNotificationLogic = sendRegistrationNotificationLogic;
            _bookWebinarMeetingLogic = bookWebinarMeetingLogic;
            _registrationCudLogic = registrationCudLogic;
        }

        public async Task Execute(
            List<Registration> registrations,
            Guid currentUserId,
            bool isSendNotificationWhenRegistrationConfirmedToLearner,
            CancellationToken cancellationToken = default)
        {
            var confirmedRegistrations = registrations.Where(p => p.IsParticipant).ToList();
            var userIds = confirmedRegistrations.Select(x => x.UserId);
            var courseIds = confirmedRegistrations.Select(x => x.CourseId);
            var currentRegistrationIds = confirmedRegistrations.Select(x => x.Id);

            var duplicateRegistrations = await _readRegistrationRepository.GetAll()
                .Where(Registration.IsPendingAdministrationExpr())
                .Where(x => userIds.Contains(x.UserId)
                            && courseIds.Contains(x.CourseId)
                            && !currentRegistrationIds.Contains(x.Id)).ToListAsync(cancellationToken);

            duplicateRegistrations.ForEach(registration =>
            {
                registration.Status = RegistrationStatus.RejectedByCA;
                registration.AdministratedBy = currentUserId;
                registration.AdministrationDate = Clock.Now;
            });

            await _registrationCudLogic.UpdateMany(duplicateRegistrations, cancellationToken);

            if (isSendNotificationWhenRegistrationConfirmedToLearner)
            {
                await _sendRegistrationNotificationLogic.SendToLearnerWhenRegistrationConfirmed(confirmedRegistrations, currentUserId);
            }

            await _bookWebinarMeetingLogic.UpdateMeeting(confirmedRegistrations, cancellationToken);
        }
    }
}
