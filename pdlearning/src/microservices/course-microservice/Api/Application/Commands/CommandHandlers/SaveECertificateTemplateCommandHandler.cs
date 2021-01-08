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
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SaveECertificateTemplateCommandHandler : BaseCommandHandler<SaveECertificateTemplateCommand>
    {
        private readonly IReadOnlyRepository<ECertificateTemplate> _readECertificateTemplateRepository;
        private readonly ECertificateTemplateCudLogic _eCertificateTemplateCudLogic;
        private readonly ValidateECertificateTemplateLogic _validateECertificateTemplateLogic;

        public SaveECertificateTemplateCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<ECertificateTemplate> readECertificateTemplateRepository,
            ValidateECertificateTemplateLogic validateECertificateTemplateLogic,
            ECertificateTemplateCudLogic eCertificateTemplateCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _eCertificateTemplateCudLogic = eCertificateTemplateCudLogic;
            _readECertificateTemplateRepository = readECertificateTemplateRepository;
            _validateECertificateTemplateLogic = validateECertificateTemplateLogic;
        }

        protected override async Task HandleAsync(SaveECertificateTemplateCommand command, CancellationToken cancellationToken)
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

        private async Task CreateNew(SaveECertificateTemplateCommand command, CancellationToken cancellationToken)
        {
            EnsureValidPermission(ECertificateTemplate.HasCreatePermission(CurrentUserId, CurrentUserRoles));

            var eCertificateTemplate = new ECertificateTemplate
            {
                Id = command.Id
            };
            SetDataForCoursePlanningCycleEntity(eCertificateTemplate, command);
            eCertificateTemplate.CreatedDate = Clock.Now;
            eCertificateTemplate.CreatedBy = CurrentUserIdOrDefault;
            eCertificateTemplate.DepartmentId = AccessControlContext.GetUserDepartment();

            await _eCertificateTemplateCudLogic.Insert(eCertificateTemplate, cancellationToken);
        }

        private async Task Update(SaveECertificateTemplateCommand command, CancellationToken cancellationToken)
        {
            var eCertificateTemplate = await _readECertificateTemplateRepository.GetAsync(command.Id);

            EnsureValidPermission(eCertificateTemplate.HasOwnerPermission(CurrentUserId, _readECertificateTemplateRepository.GetHasAdminRightChecker(eCertificateTemplate, AccessControlContext)));

            EnsureBusinessLogicValid(await _validateECertificateTemplateLogic.ValidateCanModifyAsync(eCertificateTemplate, cancellationToken));

            SetDataForCoursePlanningCycleEntity(eCertificateTemplate, command);
            eCertificateTemplate.ChangedDate = Clock.Now;
            eCertificateTemplate.UpdatedBy = CurrentUserId;

            await _eCertificateTemplateCudLogic.Update(eCertificateTemplate, cancellationToken);
        }

        private void SetDataForCoursePlanningCycleEntity(ECertificateTemplate eCertificateTemplate, SaveECertificateTemplateCommand command)
        {
            eCertificateTemplate.ECertificateLayoutId = command.ECertificateLayoutId;
            eCertificateTemplate.Title = command.Title;
            eCertificateTemplate.Params = command.Params;
            eCertificateTemplate.Status = command.Status;
        }
    }
}
