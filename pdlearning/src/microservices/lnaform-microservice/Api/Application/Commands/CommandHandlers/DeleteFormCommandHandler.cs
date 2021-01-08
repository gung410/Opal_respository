using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Application.Events;
using Microservice.LnaForm.Application.Services;
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
    public class DeleteFormCommandHandler : BaseCommandHandler<DeleteFormCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormQuestion> _formQuestionRepository;
        private readonly IRepository<FormParticipant> _formParticipantRepository;
        private readonly IFormUrlExtractor _formUrlExtractor;

        public DeleteFormCommandHandler(
            IThunderCqrs thunderCqrs,
            IRepository<FormEntity> formRepository,
            IRepository<FormQuestion> formQuestionRepository,
            IAccessControlContext accessControlContext,
            IRepository<FormParticipant> formParticipantRepository,
            IFormUrlExtractor formUrlExtractor,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _formRepository = formRepository;
            _formQuestionRepository = formQuestionRepository;
            _formUrlExtractor = formUrlExtractor;
            _formParticipantRepository = formParticipantRepository;
        }

        protected override async Task HandleAsync(DeleteFormCommand command, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository
                .GetAll()
                .ApplyAccessControlEx(AccessControlContext, LnaFormEntityExpressions.HasOwnerPermissionExpr(CurrentUserId))
                .IgnoreArchivedItems();

            var existedForm = await formQuery
                .FirstOrDefaultAsync(p => p.Id == command.FormId, cancellationToken);

            if (existedForm != null)
            {
                await _formParticipantRepository.DeleteAsync(p => p.FormId == command.FormId);
                await _formQuestionRepository.DeleteAsync(p => p.FormId == command.FormId);
                await _formRepository.DeleteAsync(command.FormId);
                await _formUrlExtractor.DeleteExtractedUrls(command.FormId);

                await _thunderCqrs.SendEvent(new FormChangeEvent(existedForm, FormChangeType.Deleted), cancellationToken);
            }
        }
    }
}
