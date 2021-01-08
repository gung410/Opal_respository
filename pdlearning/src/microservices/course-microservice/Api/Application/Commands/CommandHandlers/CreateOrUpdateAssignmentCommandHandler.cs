using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class CreateOrUpdateAssignmentCommandHandler : BaseCommandHandler<CreateOrUpdateAssignmentCommand>
    {
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly EnsureCanSaveContentLogic _ensureCanSaveContentLogic;
        private readonly ProcessPostSavingContentLogic _processPostSavingContentLogic;
        private readonly AssignmentCudLogic _assignmentCudLogic;

        public CreateOrUpdateAssignmentCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            AssignmentCudLogic assignmentCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            EnsureCanSaveContentLogic ensureCanSaveContentLogic,
            ProcessPostSavingContentLogic processPostSavingContentLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAssignmentRepository = readAssignmentRepository;
            _ensureCanSaveContentLogic = ensureCanSaveContentLogic;
            _processPostSavingContentLogic = processPostSavingContentLogic;
            _assignmentCudLogic = assignmentCudLogic;
        }

        protected override async Task HandleAsync(CreateOrUpdateAssignmentCommand command, CancellationToken cancellationToken)
        {
            if (command.IsCreated)
            {
                await CreateNew(command, cancellationToken);
            }
            else
            {
                await Update(command, cancellationToken);
            }
        }

        private async Task Update(CreateOrUpdateAssignmentCommand command, CancellationToken cancellationToken)
        {
            var assignment = await _readAssignmentRepository.GetAsync(command.Id);

            var (course, classrun) = await _ensureCanSaveContentLogic.Execute(command.CourseId, command.ClassRunId, assignment);

            assignment.ChangedBy = CurrentUserIdOrDefault;
            assignment.ChangedDate = Clock.Now;
            assignment.CourseId = command.CourseId;
            assignment.ClassRunId = command.ClassRunId;
            assignment.Type = command.Type;
            assignment.Title = command.Title;
            assignment.UpdateAssessmentConfig(command.AssessmentId, command.ScoreMode);

            await _assignmentCudLogic.Update(assignment, command.QuizForm, cancellationToken);

            await _processPostSavingContentLogic.Execute(course, classrun, CurrentUserIdOrDefault, cancellationToken);
        }

        private async Task CreateNew(CreateOrUpdateAssignmentCommand command, CancellationToken cancellationToken)
        {
            var (course, classrun) = await _ensureCanSaveContentLogic.Execute(command.CourseId, command.ClassRunId);

            var newAssignment = new Assignment
            {
                Id = command.Id,
                CourseId = command.CourseId,
                ClassRunId = command.ClassRunId,
                Type = command.Type,
                Title = command.Title,
                AssessmentConfig = command.AssessmentId != null
                    ? new AssignmentAssessmentConfig
                    {
                        AssessmentId = command.AssessmentId.Value,
                        NumberAutoAssessor = 0,
                        ScoreMode = command.ScoreMode.GetValueOrDefault()
                    }
                    : null,
                CreatedBy = CurrentUserIdOrDefault,
                CreatedDate = Clock.Now
            };

            await _assignmentCudLogic.Insert(newAssignment, command.QuizForm, cancellationToken);

            await _processPostSavingContentLogic.Execute(course, classrun, CurrentUserIdOrDefault, cancellationToken);
        }
    }
}
