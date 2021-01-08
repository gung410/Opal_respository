using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Application.Events;
using Microservice.LnaForm.Common.Extensions;
using Microservice.LnaForm.Common.Helpers;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Commands
{
    public class RollbackFormCommandHandler : BaseCommandHandler<RollbackFormCommand>
    {
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormQuestion> _formQuestionRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public RollbackFormCommandHandler(
            IRepository<FormEntity> formRepository,
            IRepository<FormQuestion> formQuestionRepository,
            IRepository<AccessRight> accessRightRepository,
            IAccessControlContext accessControlContext,
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _formRepository = formRepository;
            _formQuestionRepository = formQuestionRepository;
            _accessRightRepository = accessRightRepository;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(RollbackFormCommand command, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository
                .GetAll()
                .ApplyAccessControlEx(AccessControlContext, LnaFormEntityExpressions.HasOwnerPermissionExpr(CurrentUserId))
                .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId);

            var originalForm = await formQuery.FirstOrDefaultAsync(f => f.Id == command.RevertFromRecordId, cancellationToken);
            if (originalForm == null)
            {
                throw new FormAccessDeniedException();
            }

            var newClonedForm = F.DeepClone(originalForm).Pipe(_ =>
            {
                _.Id = command.RevertToRecordId;
                _.ChangedDate = Clock.Now;
                _.IsArchived = false;
                _.ParentId = originalForm.ParentId == Guid.Empty ? originalForm.Id : originalForm.Id;
                _.OriginalObjectId = originalForm.OriginalObjectId == Guid.Empty ? originalForm.ParentId : originalForm.OriginalObjectId;
                return _;
            });

            await _formRepository.InsertAsync(newClonedForm);

            await this.CloneFormQuestions(command.RevertFromRecordId, command.RevertToRecordId);

            await _thunderCqrs.SendEvent(new FormChangeEvent(newClonedForm, FormChangeType.Rollback), cancellationToken);
        }

        private async Task CloneFormQuestions(Guid originalFormId, Guid newClonnedFormId)
        {
            var allQuestions = await _formQuestionRepository.GetAllListAsync(_ => _.FormId == originalFormId);
            var clonedItems = allQuestions
                .Select(q =>
                {
                    q.Id = Guid.NewGuid();
                    q.FormId = newClonnedFormId;
                    return q;
                })
                .ToList();

            foreach (var formQuestion in clonedItems)
            {
                await _formQuestionRepository.InsertAsync(formQuestion);
            }
        }
    }
}
