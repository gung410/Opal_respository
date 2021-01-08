using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class AssignAssessmentCommandHandler : BaseCommandHandler<AssignAssessmentCommand>
    {
        private readonly AssessmentAnswerCudLogic _assessmentAnswerCudLogic;
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly SendAssessmentAnswerAssignedNotifyAssigneeLogic _sendAssessmentAnswerAssignedNotifyAssigneeLogic;
        private readonly SetupPeerAssessmentLogic _setupPeerAssessmentLogic;

        public AssignAssessmentCommandHandler(
            AssessmentAnswerCudLogic assessmentAnswerCudLogic,
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            SendAssessmentAnswerAssignedNotifyAssigneeLogic sendAssessmentAnswerAssignedNotifyAssigneeLogic,
            SetupPeerAssessmentLogic setupPeerAssessmentLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _assessmentAnswerCudLogic = assessmentAnswerCudLogic;
            _readAssignmentRepository = readAssignmentRepository;
            _sendAssessmentAnswerAssignedNotifyAssigneeLogic = sendAssessmentAnswerAssignedNotifyAssigneeLogic;
            _setupPeerAssessmentLogic = setupPeerAssessmentLogic;
        }

        protected override async Task HandleAsync(
            AssignAssessmentCommand command,
            CancellationToken cancellationToken)
        {
            var assignment = await _readAssignmentRepository.GetAsync(command.AssignmentId);

            EnsureBusinessLogicValid(assignment.ValidateCanUseToAssignAssessment());

            var facilitatorAssessmentAnswers = command
                .ParticipantAssignmentTrackIds
                .SelectList(participantAssignmentTrackId => new AssessmentAnswer()
                {
                    Id = Guid.NewGuid(),
                    AssessmentId = assignment.AssessmentConfig.AssessmentId,
                    ParticipantAssignmentTrackId = participantAssignmentTrackId,
                    UserId = Guid.Empty,
                    CriteriaAnswers = new List<AssessmentCriteriaAnswer>(),
                    CreatedBy = CurrentUserIdOrDefault
                });

            if (assignment.AssessmentConfig.NumberAutoAssessor > 0)
            {
                var peerAssessmentAnswers = await _setupPeerAssessmentLogic.Execute(command.ParticipantAssignmentTrackIds, assignment, cancellationToken);

                var allAssessmentAnswers = facilitatorAssessmentAnswers.Concat(peerAssessmentAnswers).ToList();

                await _assessmentAnswerCudLogic.InsertMany(allAssessmentAnswers, cancellationToken);

                await _sendAssessmentAnswerAssignedNotifyAssigneeLogic.Execute(CurrentUserIdOrDefault, allAssessmentAnswers, cancellationToken);
            }
            else
            {
                await _assessmentAnswerCudLogic.InsertMany(facilitatorAssessmentAnswers, cancellationToken);

                await _sendAssessmentAnswerAssignedNotifyAssigneeLogic.Execute(CurrentUserIdOrDefault, facilitatorAssessmentAnswers, cancellationToken);
            }
        }
    }
}
