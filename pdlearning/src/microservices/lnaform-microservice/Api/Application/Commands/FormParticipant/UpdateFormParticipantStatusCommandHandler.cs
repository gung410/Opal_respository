using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.LnaForm.Application.Commands
{
    public class UpdateFormParticipantStatusCommandHandler : BaseCommandHandler<UpdateFormParticipantStatusCommand>
    {
        private readonly IRepository<FormParticipant> _formParticipantRepository;
        private readonly IRepository<FormAnswer> _formAnswerRepository;

        public UpdateFormParticipantStatusCommandHandler(
            IAccessControlContext accessControlContext,
            IRepository<FormParticipant> formParticipantRepository,
            IRepository<FormAnswer> formAnswerRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _formParticipantRepository = formParticipantRepository;
            _formAnswerRepository = formAnswerRepository;
        }

        protected override async Task HandleAsync(UpdateFormParticipantStatusCommand command, CancellationToken cancellationToken)
        {
            var answer = _formAnswerRepository.FirstOrDefault(
                m => m.FormId == command.FormId && m.OwnerId == command.CurrentUserId);

            if (answer == null)
            {
                throw new EntityNotFoundException($"There is no answer of current request.");
            }

            var currentParticipant = await _formParticipantRepository.FirstOrDefaultAsync(
                m => m.FormId == command.FormId && m.UserId == command.CurrentUserId);

            if (currentParticipant == null)
            {
                throw new EntityNotFoundException($"There is no participant of current request.");
            }

            currentParticipant.Status = answer.IsCompleted ? FormParticipantStatus.Completed : FormParticipantStatus.Incomplete;
            await _formParticipantRepository.UpdateAsync(currentParticipant);
        }
    }
}