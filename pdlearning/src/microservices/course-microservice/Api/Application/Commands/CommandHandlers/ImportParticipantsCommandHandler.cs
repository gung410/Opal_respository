using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ImportParticipantsCommandHandler : BaseCommandHandler<ImportParticipantsCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly SendPlacementLetterLogic _sendPlacementLetterLogic;
        private readonly RegistrationCudLogic _registrationCudLogic;
        private readonly BookWebinarMeetingLogic _bookWebinarMeetingLogic;

        public ImportParticipantsCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            SendPlacementLetterLogic sendPlacementLetterLogic,
            BookWebinarMeetingLogic bookWebinarMeetingLogic,
            RegistrationCudLogic registrationCudLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _sendPlacementLetterLogic = sendPlacementLetterLogic;
            _registrationCudLogic = registrationCudLogic;
            _bookWebinarMeetingLogic = bookWebinarMeetingLogic;
        }

        protected override async Task HandleAsync(ImportParticipantsCommand command, CancellationToken cancellationToken)
        {
            var course = await _readCourseRepository.GetAllWithAccessControl(
                    AccessControlContext,
                    CourseEntity.HasAdministrationPermissionExpr(CurrentUserId, CurrentUserRoles))
                .Where(x => x.Id == command.CourseId)
                .FirstOrDefaultAsync(cancellationToken);

            EnsureEntityFound(course);

            EnsureValidPermission(course.HasImportParticipantPermission(CurrentUserId, CurrentUserRoles));

            EnsureBusinessLogicValid(course, p => p.ValidateCanImportParticipant());

            if (command.ToCreateRegistrations.Any())
            {
                command.ToCreateRegistrations.ForEach(x =>
                {
                    x.AdministratedBy = CurrentUserId;
                });
                await _registrationCudLogic.InsertMany(command.ToCreateRegistrations);
            }

            if (command.ToConfirmRegistrations.Any())
            {
                command.ToConfirmRegistrations.ForEach(x =>
                {
                    x.Status = RegistrationStatus.ConfirmedByCA;
                });
                await _registrationCudLogic.UpdateMany(command.ToConfirmRegistrations);

                // Update list participants of Webinar when class started.
                await _bookWebinarMeetingLogic.UpdateMeeting(command.ToConfirmRegistrations, cancellationToken);
            }

            if (command.ToRejectRegistrations.Any())
            {
                command.ToRejectRegistrations.ForEach(x =>
                {
                    x.Status = RegistrationStatus.RejectedByCA;
                });
                await _registrationCudLogic.UpdateMany(command.ToRejectRegistrations);
            }

            var newParticipants = command.ToCreateRegistrations.Union(command.ToConfirmRegistrations).ToList();
            await _sendPlacementLetterLogic.ByRegistrations(
                newParticipants,
                CurrentUserIdOrDefault,
                cancellationToken);
        }
    }
}
