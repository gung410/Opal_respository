using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class DeleteAnnouncementTemplateCommandHandler : BaseCommandHandler<DeleteAnnouncementTemplateCommand>
    {
        private readonly IReadOnlyRepository<AnnouncementTemplate> _readAnnouncementTemplateRepository;
        private readonly AnnouncementTemplateCudLogic _announcementTemplateCudLogic;

        public DeleteAnnouncementTemplateCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<AnnouncementTemplate> readAnnouncementTemplateRepository,
            AnnouncementTemplateCudLogic announcementTemplateCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAnnouncementTemplateRepository = readAnnouncementTemplateRepository;
            _announcementTemplateCudLogic = announcementTemplateCudLogic;
        }

        protected override async Task HandleAsync(DeleteAnnouncementTemplateCommand command, CancellationToken cancellationToken)
        {
            var announcementTemplate = await _readAnnouncementTemplateRepository.GetAsync(command.AnnouncementTemplateId);

            EnsureValidPermission(announcementTemplate.HasOwnerPermission(CurrentUserId, CurrentUserRoles));

            await _announcementTemplateCudLogic.Delete(announcementTemplate, cancellationToken);
        }
    }
}
