using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Events;
using Microservice.Form.Common.Extensions;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Infrastructure;
using Microservice.Form.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Commands
{
    public class MarkFormAsArchivedCommandHandler : BaseCommandHandler<MarkFormAsArchivedCommand>
    {
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public MarkFormAsArchivedCommandHandler(
            IRepository<FormEntity> formRepository,
            IAccessControlContext accessControlContext,
            IRepository<AccessRight> accessRightRepository,
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _formRepository = formRepository;
            _accessRightRepository = accessRightRepository;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(MarkFormAsArchivedCommand command, CancellationToken cancellationToken)
        {
            await Update(command, cancellationToken);
        }

        private async Task Update(MarkFormAsArchivedCommand command, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository
                .GetAllWithAccessControl(AccessControlContext, FormEntityExpressions.HasOwnerPermissionExpr(CurrentUserId))
                .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId)
                .IgnoreArchivedItems();

            var existedForm = await formQuery.FirstOrDefaultAsync(dc => dc.Id == command.Id, cancellationToken);

            if (existedForm == null)
            {
                throw new FormAccessDeniedException();
            }

            existedForm.IsArchived = true;
            await _formRepository.UpdateAsync(existedForm);

            await _thunderCqrs.SendEvent(new FormChangeEvent(existedForm, FormChangeType.Archived), cancellationToken);
        }
    }
}
