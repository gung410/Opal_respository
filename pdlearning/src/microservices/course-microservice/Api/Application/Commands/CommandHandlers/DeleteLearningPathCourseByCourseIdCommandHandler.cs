using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class DeleteLearningPathCourseByCourseIdCommandHandler : BaseCommandHandler<DeleteLearningPathCourseByCourseIdCommand>
    {
        private readonly LearningPathCudLogic _learningPathCudLogic;

        public DeleteLearningPathCourseByCourseIdCommandHandler(
          IUnitOfWorkManager unitOfWorkManager,
          LearningPathCudLogic learningPathCudLogic,
          IUserContext userContext,
          IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _learningPathCudLogic = learningPathCudLogic;
        }

        protected override async Task HandleAsync(DeleteLearningPathCourseByCourseIdCommand command, CancellationToken cancellationToken)
        {
            await _learningPathCudLogic.DeleteLearningPathCourseByCourseId(command.CourseId);
        }
    }
}
