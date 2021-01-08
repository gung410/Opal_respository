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
    public class DeleteCommunityEventCommandHandler : BaseCommandHandler<DeleteCommunityEventCommand>
    {
        private readonly IRepository<CommunityEvent> _communityEventRepository;
        private readonly IRepository<CommunityMembership> _communityMembershipRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public DeleteCommunityEventCommandHandler(
            IRepository<CommunityEvent> communityEventRepository,
            IRepository<CommunityMembership> communityMembershipRepository,
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWork,
            IUserContext userContext)
            : base(unitOfWork, userContext)
        {
            _communityEventRepository = communityEventRepository;
            _communityMembershipRepository = communityMembershipRepository;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(DeleteCommunityEventCommand command, CancellationToken cancellationToken)
        {
            var eventEntity = await _communityEventRepository.GetAsync(command.EventId);
            if (eventEntity == null)
            {
                throw new EntityNotFoundException(typeof(CommunityEvent), command.EventId);
            }

            if (await HasDeletePermission(eventEntity, command.UserId.Value))
            {
                await _communityEventRepository.DeleteAsync(eventEntity);

                await _thunderCqrs.SendEvent(new CommunityEventChangeEvent(eventEntity, CommunityEventChangeType.Deleted), cancellationToken);
            }
            else
            {
                throw new DeleteEventDeniedException();
            }
        }

        private async Task<bool> HasDeletePermission(CommunityEvent communityEvent, Guid userId)
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
