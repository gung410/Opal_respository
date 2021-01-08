using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SaveLearningPathCommandHandler : BaseCommandHandler<SaveLearningPathCommand>
    {
        private readonly IReadOnlyRepository<LearningPath> _readLearningPathRepository;
        private readonly LearningPathCudLogic _learningPathCudLogic;

        public SaveLearningPathCommandHandler(
          IUnitOfWorkManager unitOfWorkManager,
          IReadOnlyRepository<LearningPath> readLearningPathRepository,
          LearningPathCudLogic learningPathCudLogic,
          IUserContext userContext,
          IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readLearningPathRepository = readLearningPathRepository;
            _learningPathCudLogic = learningPathCudLogic;
        }

        protected override async Task HandleAsync(SaveLearningPathCommand command, CancellationToken cancellationToken)
        {
            if (command.IsCreateNew)
            {
                await CreateNew(command, cancellationToken);
            }
            else
            {
                await Update(command, cancellationToken);
            }
        }

        private async Task Update(SaveLearningPathCommand command, CancellationToken cancellationToken)
        {
            var currentLearningPath = await _readLearningPathRepository.FirstOrDefaultAsync(f => f.Id == command.Id && f.CreatedBy == CurrentUserId);

            currentLearningPath = EnsureEntityFound(currentLearningPath);

            EnsureValidPermission(
                LearningPath.HasCreateEditPublishUnpublishPermission(CurrentUserId, CurrentUserRoles, p => HasPermissionPrefix(p)));

            SetDataForLearningPathEntity(currentLearningPath, command);

            await _learningPathCudLogic.Update(currentLearningPath, command.ToSaveLearningPathCourses, cancellationToken);
        }

        private async Task CreateNew(SaveLearningPathCommand command, CancellationToken cancellationToken)
        {
            EnsureValidPermission(
                LearningPath.HasCreateEditPublishUnpublishPermission(CurrentUserId, CurrentUserRoles, p => HasPermissionPrefix(p)));

            var newLearningPath = new LearningPath
            {
                Id = command.Id
            };
            SetDataForLearningPathEntity(newLearningPath, command);
            newLearningPath.CreatedBy = CurrentUserIdOrDefault;
            newLearningPath.CreatedDate = Clock.Now;

            var learningPathCourses = command.ToSaveLearningPathCourses
                .Select(p => p.Create(newLearningPath.Id, p.CourseId, p.Order.GetValueOrDefault()))
                .ToList();

            await _learningPathCudLogic.Insert(newLearningPath, learningPathCourses, cancellationToken);
        }

        private void SetDataForLearningPathEntity(LearningPath learningPath, SaveLearningPathCommand command)
        {
            learningPath.Title = command.Title;
            learningPath.ThumbnailUrl = command.ThumbnailUrl;
            learningPath.Status = command.Status;
            learningPath.CourseLevelIds = command.CourseLevelIds;
            learningPath.PDAreaThemeIds = command.PDAreaThemeIds;
            learningPath.ServiceSchemeIds = command.ServiceSchemeIds;
            learningPath.SubjectAreaIds = command.SubjectAreaIds;
            learningPath.LearningFrameworkIds = command.LearningFrameworkIds;
            learningPath.LearningDimensionIds = command.LearningDimensionIds;
            learningPath.LearningAreaIds = command.LearningAreaIds;
            learningPath.LearningSubAreaIds = command.LearningSubAreaIds;
            learningPath.MetadataKeys = command.MetadataKeys;
        }
    }
}
