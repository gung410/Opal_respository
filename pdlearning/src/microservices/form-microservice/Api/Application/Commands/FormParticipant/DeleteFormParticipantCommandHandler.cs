using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Events;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.Services;
using Microservice.Form.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Commands
{
    public class DeleteFormParticipantCommandHandler : BaseCommandHandler<DeleteFormParticipantCommand>
    {
        private readonly IRepository<FormParticipant> _formParticipantRepository;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly FormNotifyApplicationService _formParticipantNotifyApplicationService;
        private readonly IThunderCqrs _thunderCqrs;

        public DeleteFormParticipantCommandHandler(
            IRepository<FormParticipant> formParticipantRepository,
            IRepository<FormEntity> formRepository,
            IRepository<UserEntity> userRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext,
            FormNotifyApplicationService formParticipantNotifyApplicationService,
            IThunderCqrs thunderCqrs) : base(unitOfWorkManager, accessControlContext)
        {
            _formParticipantRepository = formParticipantRepository;
            _formRepository = formRepository;
            _userRepository = userRepository;
            _formParticipantNotifyApplicationService = formParticipantNotifyApplicationService;
            _thunderCqrs = thunderCqrs;
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

            await PerformNotifyRemoveParticipant(participants, existedForm, cancellationToken);
        }

        private async Task PerformNotifyRemoveParticipant(List<FormParticipant> participants, FormEntity form, CancellationToken cancellationToken)
        {
            var participantIds = participants.Select(p => p.UserId).ToList();

            var userInfoDict = await _userRepository
                .GetAll()
                .Where(p => participantIds.Contains(p.Id) || p.Id == form.OwnerId)
                .Select(p => new
                {
                    p.Id,
                    FullName = p.FullName()
                })
                .ToDictionaryAsync(
                    key => key.Id,
                    value => new
                    {
                        value.Id,
                        value.FullName
                    },
                    cancellationToken);

            var formOwnerName = userInfoDict[form.OwnerId].FullName;

            foreach (var participant in participants)
            {
                await _formParticipantNotifyApplicationService.NotifyRemovedFormParticipant(
                    new NotifyFormParticipantModel
                    {
                        FormOriginalObjectId = form.OriginalObjectId,
                        FormOwnerName = formOwnerName,
                        FormTitle = form.Title,
                        ParcitipantId = userInfoDict[participant.UserId].Id,
                        ParticipantName = userInfoDict[participant.UserId].FullName
                    });

                await _thunderCqrs.SendEvent(
                    new FormParticipantChangeEvent(participant, FormParticipantChangeType.Deleted),
                    cancellationToken);
            }
        }
    }
}
