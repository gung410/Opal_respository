using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class DeleteAssessmentAnswerCommandHandler : BaseCommandHandler<DeleteAssessmentAnswerCommand>
    {
        private readonly IReadOnlyRepository<AssessmentAnswer> _readAssessmentAnswerRepository;
        private readonly AssessmentAnswerCudLogic _assessmentAnswerCudLogic;
        private readonly SetupPeerAssessmentLogic _setupPeerAssessmentLogic;

        public DeleteAssessmentAnswerCommandHandler(
            IReadOnlyRepository<AssessmentAnswer> readAssessmentAnswerRepository,
            AssessmentAnswerCudLogic assessmentAnswerCudLogic,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            SetupPeerAssessmentLogic setupPeerAssessmentLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAssessmentAnswerRepository = readAssessmentAnswerRepository;
            _assessmentAnswerCudLogic = assessmentAnswerCudLogic;
            _setupPeerAssessmentLogic = setupPeerAssessmentLogic;
        }

        protected override async Task HandleAsync(DeleteAssessmentAnswerCommand command, CancellationToken cancellationToken)
        {
            var assessmentAnswer = await _readAssessmentAnswerRepository.GetAsync(command.Id);

            EnsureBusinessLogicValid(assessmentAnswer.ValidateCanDelete());

            await _assessmentAnswerCudLogic.Delete(assessmentAnswer, cancellationToken);
            await _setupPeerAssessmentLogic.UpdateAssignedAssessmentManuallyOnceTrue(assessmentAnswer.ParticipantAssignmentTrackId);
        }
    }
}
