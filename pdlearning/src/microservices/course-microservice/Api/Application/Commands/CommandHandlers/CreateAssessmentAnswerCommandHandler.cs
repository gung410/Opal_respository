using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class CreateAssessmentAnswerCommandHandler : BaseCommandHandler<CreateAssessmentAnswerCommand>
    {
        private readonly IReadOnlyRepository<AssessmentAnswer> _readAssessmentAnswerRepository;
        private readonly SendAssessmentAnswerAssignedNotifyAssigneeLogic _sendAssessmentAnswerAssignedNotifyAssigneeLogic;
        private readonly AssessmentAnswerCudLogic _assessmentAnswerCudLogic;
        private readonly SetupPeerAssessmentLogic _setupPeerAssessmentLogic;

        public CreateAssessmentAnswerCommandHandler(
            IReadOnlyRepository<AssessmentAnswer> readAssessmentAnswerRepository,
            SendAssessmentAnswerAssignedNotifyAssigneeLogic sendAssessmentAnswerAssignedNotifyAssigneeLogic,
            AssessmentAnswerCudLogic assessmentAnswerCudLogic,
            IAccessControlContext<CourseUser> accessControlContext,
            IUserContext userContext,
            IUnitOfWorkManager unitOfWorkManager,
            SetupPeerAssessmentLogic setupPeerAssessmentLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAssessmentAnswerRepository = readAssessmentAnswerRepository;
            _sendAssessmentAnswerAssignedNotifyAssigneeLogic = sendAssessmentAnswerAssignedNotifyAssigneeLogic;
            _assessmentAnswerCudLogic = assessmentAnswerCudLogic;
            _setupPeerAssessmentLogic = setupPeerAssessmentLogic;
        }

        protected override async Task HandleAsync(CreateAssessmentAnswerCommand command, CancellationToken cancellationToken)
        {
            EnsureBusinessLogicValid(ValidateCanCreateAssessmentAnswer(command));

            await CreateNewAssessmentAnswer(command, cancellationToken);
            await _setupPeerAssessmentLogic.UpdateAssignedAssessmentManuallyOnceTrue(command.ParticipantAssignmentTrackId);
        }

        private async Task CreateNewAssessmentAnswer(CreateAssessmentAnswerCommand command, CancellationToken cancellationToken)
        {
            var newAssessmentAnswer = new AssessmentAnswer
            {
                Id = command.Id,
                AssessmentId = command.AssessmentId,
                ParticipantAssignmentTrackId = command.ParticipantAssignmentTrackId,
                UserId = command.UserId,
                CreatedBy = CurrentUserIdOrDefault,
            };

            await _assessmentAnswerCudLogic.Insert(newAssessmentAnswer, cancellationToken);

            await _sendAssessmentAnswerAssignedNotifyAssigneeLogic.Execute(
                CurrentUserIdOrDefault,
                new List<AssessmentAnswer>() { newAssessmentAnswer },
                cancellationToken);
        }

        private Validation ValidateCanCreateAssessmentAnswer(CreateAssessmentAnswerCommand command)
        {
            var isAssignedAssessmentExisted = _readAssessmentAnswerRepository
                .GetAll()
                .Any(AssessmentAnswer.IsAssignedPeerAssessmentExpr(command.ParticipantAssignmentTrackId, command.UserId));

            if (isAssignedAssessmentExisted)
            {
                return Validation.ValidIf(false, "Cannot create because peer assessor existed.");
            }

            return Validation.Valid();
        }
    }
}
