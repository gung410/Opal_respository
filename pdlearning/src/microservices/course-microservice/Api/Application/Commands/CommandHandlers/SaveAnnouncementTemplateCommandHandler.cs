using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SaveAnnouncementTemplateCommandHandler : BaseCommandHandler<SaveAnnouncementTemplateCommand>
    {
        private readonly IReadOnlyRepository<AnnouncementTemplate> _readAnnouncementTemplateRepository;
        private readonly AnnouncementTemplateCudLogic _announcementTemplateCudLogic;

        public SaveAnnouncementTemplateCommandHandler(
           IReadOnlyRepository<AnnouncementTemplate> readAnnouncementTemplateRepository,
           IUnitOfWorkManager unitOfWorkManager,
           AnnouncementTemplateCudLogic announcementTemplateCudLogic,
           IUserContext userContext,
           IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAnnouncementTemplateRepository = readAnnouncementTemplateRepository;
            _announcementTemplateCudLogic = announcementTemplateCudLogic;
        }

        protected override async Task HandleAsync(SaveAnnouncementTemplateCommand command, CancellationToken cancellationToken)
        {
            if (command.IsCreate)
            {
                await CreateNew(command, cancellationToken);
            }
            else
            {
                await Update(command, cancellationToken);
            }
        }

        private async Task CreateNew(SaveAnnouncementTemplateCommand command, CancellationToken cancellationToken)
        {
            EnsureValidPermission(AnnouncementTemplate.HasCreatePermission(CurrentUserId, CurrentUserRoles));

            var announcementTemplate = new AnnouncementTemplate
            {
                Id = command.Id
            };

            SetDataForAnnouncementTemplate(announcementTemplate, command);
            announcementTemplate.CreatedBy = CurrentUserId;

            await _announcementTemplateCudLogic.Insert(announcementTemplate, cancellationToken);

            var existedSameTitleTemplate =
                await _readAnnouncementTemplateRepository.FirstOrDefaultAsync(p => p.Title == announcementTemplate.Title);
            if (existedSameTitleTemplate != null)
            {
                await _announcementTemplateCudLogic.Delete(announcementTemplate, cancellationToken);
            }
        }

        private async Task Update(SaveAnnouncementTemplateCommand command, CancellationToken cancellationToken)
        {
            var announcementTemplate = await _readAnnouncementTemplateRepository.GetAsync(command.Id);

            EnsureValidPermission(announcementTemplate.HasOwnerPermission(CurrentUserId, CurrentUserRoles));

            SetDataForAnnouncementTemplate(announcementTemplate, command);
            announcementTemplate.ChangedDate = Clock.Now;
            announcementTemplate.ChangedBy = CurrentUserId;

            await _announcementTemplateCudLogic.Update(announcementTemplate, cancellationToken);
        }

        private void SetDataForAnnouncementTemplate(AnnouncementTemplate announcement, SaveAnnouncementTemplateCommand command)
        {
            announcement.Title = command.Title;
            announcement.Message = command.Message;
        }
    }
}
