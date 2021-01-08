using System;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class DeletePersonalEventCommandHandler : BaseCommandHandler<DeletePersonalEventCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly IRepository<UserPersonalEvent> _userEventRepository;

        public DeletePersonalEventCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IRepository<UserPersonalEvent> userEventRepository,
            IUnitOfWorkManager unitOfWork,
            IUserContext userContext)
            : base(unitOfWork, userContext)
        {
            _personalEventRepository = personalEventRepository;
            _userEventRepository = userEventRepository;
        }

        protected override async Task HandleAsync(DeletePersonalEventCommand command, CancellationToken cancellationToken)
        {
            var eventEntity = await _personalEventRepository.GetAsync(command.EventId);
            if (eventEntity == null)
            {
                throw new EntityNotFoundException(typeof(PersonalEvent), command.EventId);
            }

            if (!command.UserId.HasValue || IsEventOwner(eventEntity, command.UserId.Value))
            {
                await DeleteEventAndInvitedAttendees(eventEntity);
            }
            else
            {
                await HideEventFromAttendee(command.EventId, command.UserId.Value);
            }
        }

        private bool IsEventOwner(PersonalEvent eventEntity, Guid userId)
        {
            return eventEntity.CreatedBy == userId;
        }

        private async Task HideEventFromAttendee(Guid eventId, Guid userId)
        {
            var userEvent = await _userEventRepository.FirstOrDefaultAsync(ue => ue.EventId == eventId && ue.UserId == userId);
            if (userEvent == null)
            {
                throw new EntityNotFoundException(typeof(UserPersonalEvent), userId);
            }

            userEvent.IsAccepted = false;

            await _userEventRepository.UpdateAsync(userEvent);
        }

        private async Task DeleteEventAndInvitedAttendees(PersonalEvent eventEntity)
        {
            await _personalEventRepository.DeleteAsync(eventEntity);

            var userEvents = await _userEventRepository.GetAllListAsync(ue => ue.EventId == eventEntity.Id);
            await _userEventRepository.DeleteManyAsync(userEvents);
        }
    }
}
