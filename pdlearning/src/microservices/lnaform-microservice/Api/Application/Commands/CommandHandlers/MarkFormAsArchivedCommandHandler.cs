using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Application.Events;
using Microservice.LnaForm.Common.Extensions;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Infrastructure;
using Microservice.LnaForm.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Commands
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
                .GetAll()
                .ApplyAccessControlEx(AccessControlContext, LnaFormEntityExpressions.HasOwnerPermissionExpr(CurrentUserId))
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
