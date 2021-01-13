using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.LnaForm.Application.Commands
{
    public class AssignFormParticipantCommandHandler : BaseCommandHandler<AssignFormParticipantCommand>
    {
        private readonly IRepository<FormParticipant> _formParticipantRepository;

        public AssignFormParticipantCommandHandler(
            IAccessControlContext accessControlContext,
            IRepository<FormParticipant> formParticipantRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _formParticipantRepository = formParticipantRepository;
        }

        protected override async Task HandleAsync(AssignFormParticipantCommand command, CancellationToken cancellationToken)
        {
            if (!command.UserIds.Any())
            {
                throw new EntityNotFoundException($"There is no participants in the {nameof(AssignFormParticipantCommand)}.");
            }

            var assignedParticipants = _formParticipantRepository
               .GetAll()
               .Where(x => command.UserIds.Contains(x.UserId) && x.FormId == command.FormId)
               .Select(x => x.UserId)
               .ToList();

            var newParticipant = command.UserIds
                .Where(uid => !assignedParticipants.Contains(uid))
                .Select(uid => new FormParticipant
                {
                    Id = Guid.NewGuid(),
                    UserId = uid,
                    FormId = command.FormId,
                    FormOriginalObjectId = command.FormOriginalObjectId,
                    IsStarted = command.IsStarted,
                    AssignedDate = Clock.Now,
                    CreatedBy = command.CurrentUserId,
                    Status = command.Status ?? FormParticipantStatus.NotStarted
                })
                .ToList();

            await _formParticipantRepository.InsertManyAsync(newParticipant);
        }
    }
}