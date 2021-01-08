using System;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Events;
using Microservice.Calendar.Application.Exceptions;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class UpdateCommunityEventCommandHandler : BaseCommandHandler<UpdateCommunityEventCommand>
    {
        private readonly IRepository<CommunityEvent> _communityEventRepository;
        private readonly IRepository<CommunityMembership> _communityMembershipRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public UpdateCommunityEventCommandHandler(
            IRepository<CommunityEvent> communityEventRepository,
            IRepository<CommunityMembership> communityMembershipRepository,
            IUnitOfWorkManager unitOfWork,
            IThunderCqrs thunderCqrs,
            IUserContext userContext)
            : base(unitOfWork, userContext)
        {
            _communityEventRepository = communityEventRepository;
            _communityMembershipRepository = communityMembershipRepository;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(UpdateCommunityEventCommand command, CancellationToken cancellationToken)
        {
            var eventEntity = await _communityEventRepository.GetAsync(command.Request.Id);
            if (eventEntity == null)
            {
                throw new EntityNotFoundException(typeof(CommunityEvent), command.Request.Id);
            }

            if (!command.UserId.HasValue || !await HasEditPermission(eventEntity, command.UserId.Value))
            {
                throw new UpdateEventDeniedException();
            }

            eventEntity.Description = command.Request.Description;
            eventEntity.EndAt = command.Request.EndAt;
            eventEntity.IsAllDay = command.Request.IsAllDay;
            eventEntity.StartAt = command.Request.StartAt;
            eventEntity.Title = command.Request.Title;
            eventEntity.Source = command.Request.CalendarEventSource;
            eventEntity.RepeatFrequency = command.Request.RepeatFrequency;
            eventEntity.RepeatUntil = command.Request.RepeatUntil;
            if (command.Request.CommunityEventPrivacy.HasValue)
            {
                eventEntity.CommunityEventPrivacy = command.Request.CommunityEventPrivacy.Value;
            }

            await _communityEventRepository.UpdateAsync(eventEntity);

            await _thunderCqrs.SendEvent(new CommunityEventChangeEvent(eventEntity, CommunityEventChangeType.Updated), cancellationToken);
        }

        private async Task<bool> HasEditPermission(CommunityEvent communityEvent, Guid userId)
        {
            var membership = await _communityMembershipRepository
                .FirstOrDefaultAsync(x => x.CommunityId == communityEvent.CommunityId && x.UserId == userId);

            if (membership == null)
            {
                return false;
            }

            return membership.Role == CommunityMembershipRole.Owner || membership.Role == CommunityMembershipRole.Admin;
        }
    }
}
