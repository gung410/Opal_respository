using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SetupPeerAssessmentCommandHandler : BaseCommandHandler<SetupPeerAssessmentCommand>
    {
        private readonly IReadOnlyRepository<Assignment> _assignmentTrackRepository;
        private readonly IReadOnlyRepository<ParticipantAssignmentTrack> _readParticipantAssignmentTrackRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly SetupPeerAssessmentLogic _setupPeerAssessmentLogic;
        private readonly AssignmentCudLogic _assignmentCudLogic;
        private readonly AssessmentAnswerCudLogic _assessmentAnswerCudLogic;

        public SetupPeerAssessmentCommandHandler(
            IReadOnlyRepository<Assignment> assignmentTrackRepository,
            IReadOnlyRepository<ParticipantAssignmentTrack> readParticipantAssignmentTrackRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            SetupPeerAssessmentLogic setupPeerAssessmentLogic,
            AssessmentAnswerCudLogic assessmentAnswerCudLogic,
            AssignmentCudLogic assignmentCudLogic,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _assignmentTrackRepository = assignmentTrackRepository;
            _readParticipantAssignmentTrackRepository = readParticipantAssignmentTrackRepository;
            _readRegistrationRepository = readRegistrationRepository;
            _setupPeerAssessmentLogic = setupPeerAssessmentLogic;
            _assignmentCudLogic = assignmentCudLogic;
            _assessmentAnswerCudLogic = assessmentAnswerCudLogic;
        }

        protected override async Task HandleAsync(SetupPeerAssessmentCommand command, CancellationToken cancellationToken)
        {
            if (command.NumberAutoAssessor > 0)
            {
                var assignment = await _assignmentTrackRepository.GetAsync(command.AssignmentId);

                await UpdateNumberAutoAssessor(command.NumberAutoAssessor, assignment, cancellationToken);

                await SetupPeerAssessment(command.ClassrunId, command.AssignmentId, assignment, cancellationToken);
            }
        }

        private async Task SetupPeerAssessment(Guid classrunId, Guid assignmentId, Assignment assignment, CancellationToken cancellationToken)
        {
            var allParticipantsInClassQuery = _readRegistrationRepository
                .GetAll()
                .Where(p => p.ClassRunId == classrunId)
                .Where(Registration.IsParticipantExpr());

            var participantAssignmentTrackIds = await _readParticipantAssignmentTrackRepository
                .GetAll()
                .Where(p => p.AssignmentId == assignmentId)
                .Where(p => allParticipantsInClassQuery.Select(p => p.Id).Contains(p.RegistrationId))
                .Where(p => p.IsAssignedAssessmentManuallyOnce == false && p.IsAutoAssignedOnce == false)
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            if (participantAssignmentTrackIds.Count > 0)
            {
                var peerAssessmentAnswers = await _setupPeerAssessmentLogic.Execute(participantAssignmentTrackIds, assignment, cancellationToken);

                await _assessmentAnswerCudLogic.InsertMany(peerAssessmentAnswers.ToList());
            }
        }

        private async Task UpdateNumberAutoAssessor(int numberAutoAssessor, Assignment assignment, CancellationToken cancellationToken)
        {
            assignment.AssessmentConfig.NumberAutoAssessor = numberAutoAssessor;
            await _assignmentCudLogic.Update(assignment, null, cancellationToken);
        }
    }
}
