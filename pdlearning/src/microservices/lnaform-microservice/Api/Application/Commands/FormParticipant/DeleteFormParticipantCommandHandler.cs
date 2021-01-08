using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Application.Services;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Infrastructure;
using Microservice.LnaForm.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Commands
{
    public class DeleteFormParticipantCommandHandler : BaseCommandHandler<DeleteFormParticipantCommand>
    {
        private readonly IRepository<FormParticipant> _formParticipantRepository;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly FormParticipantNotifyApplicationService _formParticipantNotifyApplicationService;

        public DeleteFormParticipantCommandHandler(
            IRepository<FormParticipant> formParticipantRepository,
            IRepository<FormEntity> formRepository,
            IRepository<UserEntity> userRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext,
            FormParticipantNotifyApplicationService formParticipantNotifyApplicationService) : base(unitOfWorkManager, accessControlContext)
        {
            _formParticipantRepository = formParticipantRepository;
            _formRepository = formRepository;
            _userRepository = userRepository;
            _formParticipantNotifyApplicationService = formParticipantNotifyApplicationService;
        }

        protected override async Task HandleAsync(DeleteFormParticipantCommand command, CancellationToken cancellationToken)
        {
            var existedForm = await _formRepository
                .FirstOrDefaultAsync(p => p.Id == command.FormId);

            if (existedForm == null)
            {
                throw new FormAccessDeniedException();
            }

            var participants = await _formParticipantRepository
                .GetAllListAsync(_ => command.Ids.Contains(_.Id));

            await _formParticipantRepository.DeleteManyAsync(participants);

            await PerformNotifyRemoveParticipant(participants, existedForm);
        }

        private async Task PerformNotifyRemoveParticipant(List<FormParticipant> participants, FormEntity form)
        {
            var formOwnerName = _userRepository
                .FirstOrDefault(p => p.Id == form.OwnerId)
                .FullName();

            var participantIds = participants.Select(p => p.UserId).ToList();

            var participantInfo = _userRepository
                .GetAll()
                .Where(p => participantIds.Contains(p.Id))
                .Select(p => new { p.Id, FullName = p.FullName() })
                .ToList();

            foreach (var user in participantInfo)
            {
                await _formParticipantNotifyApplicationService.NotifyRemovedFormParticipant(
                  new NotifyFormParticipantModel
                  {
                      FormOriginalObjectId = form.OriginalObjectId,
                      FormOwnerName = formOwnerName,
                      FormTitle = form.Title,
                      ParcitipantId = user.Id,
                      ParticipantName = user.FullName
                  });
            }
        }
    }
}
