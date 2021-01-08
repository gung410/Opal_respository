using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Commands
{
    public class DeleteECertificateTemplateCommandHandler : BaseCommandHandler<DeleteECertificateTemplateCommand>
    {
        private readonly ECertificateTemplateCudLogic _eCertificateTemplateCudLogic;
        private readonly IReadOnlyRepository<ECertificateTemplate> _readECertificateTemplateRepository;
        private readonly ValidateECertificateTemplateLogic _validateECertificateTemplateLogic;

        public DeleteECertificateTemplateCommandHandler(
            IReadOnlyRepository<ECertificateTemplate> readECertificateTemplateRepository,
            ValidateECertificateTemplateLogic validateECertificateTemplateLogic,
            IUnitOfWorkManager unitOfWorkManager,
            ECertificateTemplateCudLogic eCertificateTemplateCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _eCertificateTemplateCudLogic = eCertificateTemplateCudLogic;
            _readECertificateTemplateRepository = readECertificateTemplateRepository;
            _validateECertificateTemplateLogic = validateECertificateTemplateLogic;
        }

        protected override async Task HandleAsync(DeleteECertificateTemplateCommand command, CancellationToken cancellationToken)
        {
            var ecertificateTemplate = await _readECertificateTemplateRepository.GetAsync(command.ECertificateTemplateId);

            EnsureValidPermission(ecertificateTemplate.HasOwnerPermission(CurrentUserId, _readECertificateTemplateRepository.GetHasAdminRightChecker(ecertificateTemplate, AccessControlContext)));

            EnsureBusinessLogicValid(await _validateECertificateTemplateLogic.ValidateCanModifyAsync(ecertificateTemplate, cancellationToken));

            await _eCertificateTemplateCudLogic.Delete(ecertificateTemplate, cancellationToken);
        }
    }
}
