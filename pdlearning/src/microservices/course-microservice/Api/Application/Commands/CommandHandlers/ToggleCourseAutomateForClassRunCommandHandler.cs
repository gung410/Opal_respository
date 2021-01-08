using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ToggleCourseAutomateForClassRunCommandHandler : BaseCommandHandler<ToggleCourseAutomateForClassRunCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly GetAggregatedClassRunSharedQuery _getAggregatedClassRunSharedQuery;
        private readonly ClassRunCudLogic _classRunCudLogic;
        private readonly RegistrationCudLogic _registrationCudLogic;
        private readonly ProcessAutomateParticipantSelectionLogic _processAutomateParticipantSelectionLogic;

        public ToggleCourseAutomateForClassRunCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            ProcessAutomateParticipantSelectionLogic processAutomateParticipantSelectionLogic,
            ClassRunCudLogic classRunCudLogic,
            RegistrationCudLogic registrationCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            GetAggregatedClassRunSharedQuery getAggregatedClassRunSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _processAutomateParticipantSelectionLogic = processAutomateParticipantSelectionLogic;
            _classRunCudLogic = classRunCudLogic;
            _registrationCudLogic = registrationCudLogic;
            _getAggregatedClassRunSharedQuery = getAggregatedClassRunSharedQuery;
        }

        protected override async Task HandleAsync(ToggleCourseAutomateForClassRunCommand command, CancellationToken cancellationToken)
        {
            var aggregatedClassRun = await _getAggregatedClassRunSharedQuery.ByClassRunId(command.Id);

            EnsureValidPermission(
                aggregatedClassRun.Course.HasSaveCourseAutomatePermission(
                    CurrentUserId,
                    CurrentUserRoles,
                    _readCourseRepository.GetHasAdminRightChecker(F.List(aggregatedClassRun.Course), AccessControlContext)));

            EnsureBusinessLogicValid(aggregatedClassRun.Course.ValidateNotArchived());

            aggregatedClassRun.ClassRun.CourseAutomateActivated = command.CourseAutomateActivated;

            // Process automate participant selection if classrun of each registration have available slots and course automate activated.
            var registrationAutomateSelections = aggregatedClassRun.ClassRun.CourseAutomateActivated ?
               await _processAutomateParticipantSelectionLogic.ByClassRunIds(F.List(aggregatedClassRun.ClassRun.Id), null, CurrentUserIdOrDefault) :
               new List<Registration>();

            await _classRunCudLogic.Update(aggregatedClassRun, cancellationToken);

            await _registrationCudLogic.UpdateMany(registrationAutomateSelections, cancellationToken);
        }
    }
}
