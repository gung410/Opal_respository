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
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class UpdateRegistrationLearningInfoCommandHandler : BaseCommandHandler<UpdateRegistrationLearningInfoCommand>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly GetAggregatedClassRunSharedQuery _getAggregatedClassRunSharedQuery;
        private readonly AutoProcessLearningProgressLogic _autoProcessLearningProgressLogic;
        private readonly RegistrationCudLogic _registrationCudLogic;
        private readonly ProcessJustLearningCompletedParticipantLogic _processJustLearningCompletedParticipantLogic;
        private readonly ProcessJustLearningFailedParticipantLogic _processJustLearningFailedParticipantLogic;

        public UpdateRegistrationLearningInfoCommandHandler(
           IUnitOfWorkManager unitOfWorkManager,
           IReadOnlyRepository<Registration> readRegistrationRepository,
           IReadOnlyRepository<CourseEntity> readCourseRepository,
           IUserContext userContext,
           GetAggregatedClassRunSharedQuery getAggregatedClassRunSharedQuery,
           AutoProcessLearningProgressLogic autoProcessLearningProgressLogic,
           RegistrationCudLogic registrationCudLogic,
           IAccessControlContext<CourseUser> accessControlContext,
           ProcessJustLearningCompletedParticipantLogic processJustLearningCompletedParticipantLogic,
           ProcessJustLearningFailedParticipantLogic processJustLearningFailedParticipantLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _readCourseRepository = readCourseRepository;
            _getAggregatedClassRunSharedQuery = getAggregatedClassRunSharedQuery;
            _autoProcessLearningProgressLogic = autoProcessLearningProgressLogic;
            _registrationCudLogic = registrationCudLogic;
            _processJustLearningCompletedParticipantLogic = processJustLearningCompletedParticipantLogic;
            _processJustLearningFailedParticipantLogic = processJustLearningFailedParticipantLogic;
        }

        protected override async Task HandleAsync(UpdateRegistrationLearningInfoCommand command, CancellationToken cancellationToken)
        {
            if (CurrentUserId != null && command.ClassRunId != null)
            {
                var aggregatedClassRun = await _getAggregatedClassRunSharedQuery.ByClassRunId(command.ClassRunId.Value);

                EnsureValidPermission(Registration.CanManageRegistrations(
                    CurrentUserId,
                    aggregatedClassRun.Course,
                    aggregatedClassRun.ClassRun,
                    CurrentUserRoles,
                    _readCourseRepository.GetHasAdminRightChecker(aggregatedClassRun.Course, AccessControlContext)));
            }

            var registrations = await _readRegistrationRepository.GetAll()
                .WhereIf(command.ClassRunId.HasValue, a => a.ClassRunId == command.ClassRunId)
                .Where(a => command.RegistrationIds.Contains(a.Id))
                .ToListAsync(cancellationToken);

            EnsureBusinessLogicValid(registrations, p => p.ValidateCanUpdateLearningStatus(command.LearningStatus));

            var withPrevDataUpdatedRegistrations =
                registrations
                    .SelectList(registration =>
                    {
                        var prevLearningStatus = registration.LearningStatus;
                        registration.UpdateLearningStatus(command.LearningStatus);
                        registration.LearningContentProgress = command.LearningContentProgress ?? registration.LearningContentProgress;
                        return new { Registration = registration, PrevLearningStatus = prevLearningStatus };
                    });

            // If learner just started to learn or learning content progress updated, process auto update learning status progress
            if (command.LearningStatus == LearningStatus.InProgress || command.LearningContentProgress != null)
            {
                await _autoProcessLearningProgressLogic.ExecuteAsync(
                    withPrevDataUpdatedRegistrations.SelectList(p => p.Registration),
                    cancellationToken: cancellationToken);
            }

            var updatedRegistrations = withPrevDataUpdatedRegistrations.SelectList(p => p.Registration);

            await _registrationCudLogic.UpdateMany(updatedRegistrations, cancellationToken);

            var justLearningCompletedRegistrations = updatedRegistrations
                .Where(p => p.LearningStatus == LearningStatus.Completed)
                .ToList();
            await _processJustLearningCompletedParticipantLogic.Execute(justLearningCompletedRegistrations, cancellationToken);

            var revertedToFailedFromCompletedRegistrations = withPrevDataUpdatedRegistrations
                .Where(p => p.Registration.LearningStatus == LearningStatus.Failed && p.PrevLearningStatus == LearningStatus.Completed)
                .SelectList(p => p.Registration);
            await _processJustLearningFailedParticipantLogic.Execute(revertedToFailedFromCompletedRegistrations, cancellationToken);
        }
    }
}
