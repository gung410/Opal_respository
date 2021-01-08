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

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class DeleteSectionCommandHandler : BaseCommandHandler<DeleteSectionCommand>
    {
        private readonly IReadOnlyRepository<Section> _readSectionRepository;
        private readonly EnsureCanSaveContentLogic _ensureCanSaveContentLogic;
        private readonly ProcessPostSavingContentLogic _processPostSavingContentLogic;
        private readonly SectionCudLogic _sectionCudLogic;

        public DeleteSectionCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Section> readSectionRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            EnsureCanSaveContentLogic ensureCanSaveContentLogic,
            SectionCudLogic sectionCudLogic,
            ProcessPostSavingContentLogic processPostSavingContentLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readSectionRepository = readSectionRepository;
            _sectionCudLogic = sectionCudLogic;
            _ensureCanSaveContentLogic = ensureCanSaveContentLogic;
            _processPostSavingContentLogic = processPostSavingContentLogic;
        }

        protected override async Task HandleAsync(DeleteSectionCommand command, CancellationToken cancellationToken)
        {
            var section = await _readSectionRepository.GetAsync(command.SectionId);
            var (course, classRun) = await _ensureCanSaveContentLogic.Execute(section.CourseId, section.ClassRunId, section);

            await _sectionCudLogic.Delete(section);

            await _processPostSavingContentLogic.Execute(course, classRun, CurrentUserIdOrDefault, cancellationToken);
        }
    }
}
