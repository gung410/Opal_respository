using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class DeleteAssignmentCommandHandler : BaseCommandHandler<DeleteAssignmentCommand>
    {
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly ExtractCourseUrlLogic _extractCourseUrlLogic;
        private readonly EnsureCanSaveContentLogic _ensureCanSaveContentLogic;
        private readonly ProcessPostSavingContentLogic _updateContentStatusAfterSavingContent;
        private readonly AssignmentCudLogic _assignmentCudLogic;

        public DeleteAssignmentCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            ExtractCourseUrlLogic extractCourseUrlLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            EnsureCanSaveContentLogic ensureCanSaveContentLogic,
            ProcessPostSavingContentLogic updateContentStatusAfterSavingContent,
            AssignmentCudLogic assignmentCudLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _extractCourseUrlLogic = extractCourseUrlLogic;
            _ensureCanSaveContentLogic = ensureCanSaveContentLogic;
            _updateContentStatusAfterSavingContent = updateContentStatusAfterSavingContent;
            _readAssignmentRepository = readAssignmentRepository;
            _assignmentCudLogic = assignmentCudLogic;
        }

        protected override async Task HandleAsync(DeleteAssignmentCommand command, CancellationToken cancellationToken)
        {
            var assignment = await _readAssignmentRepository.GetAsync(command.Id);
            var (course, classRun) = await _ensureCanSaveContentLogic.Execute(assignment.CourseId, assignment.ClassRunId);

            await _assignmentCudLogic.Delete(assignment, cancellationToken);
            await _extractCourseUrlLogic.DeleteExtractedUrls(new List<Guid>() { assignment.Id });
            await _updateContentStatusAfterSavingContent.Execute(course, classRun, CurrentUserIdOrDefault, cancellationToken);
        }
    }
}
