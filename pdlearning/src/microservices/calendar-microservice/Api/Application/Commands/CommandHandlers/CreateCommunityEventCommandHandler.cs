using System;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Events;
using Microservice.Calendar.Application.Exceptions;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microservice.Calendar.Domain.Extensions;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class CreateCommunityEventCommandHandler : BaseCommandHandler<CreateCommunityEventCommand>
    {
        private readonly IRepository<CommunityEvent> _communityEventRepository;
        private readonly IRepository<CommunityMembership> _communityMembershipRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public CreateCommunityEventCommandHandler(
            IRepository<CommunityEvent> communityEventRepository,
            IRepository<CommunityMembership> communityMembershipRepository,
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _communityEventRepository = communityEventRepository;
            _communityMembershipRepository = communityMembershipRepository;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(CreateCommunityEventCommand command, CancellationToken cancellationToken)
        {
            if (!await HasCreationPermission(command.CreationRequest.CommunityId, command.CreatedBy))
            {
                throw new CreateEventDeniedException();
            }

            var newEvent = new CommunityEvent(command.CreationRequest.Id)
                .WithBasicInfo(
                    title: command.CreationRequest.Title,
                    description: command.CreationRequest.Description,
                    communityId: command.CreationRequest.CommunityId)
                .WithTime(
                    startAt: command.CreationRequest.StartAt,
                    endAt: command.CreationRequest.EndAt,
                    repeatUntil: command.CreationRequest.RepeatUntil,
                    repeatFrequency: command.CreationRequest.RepeatFrequency,
                    isAllDay: command.CreationRequest.IsAllDay)
                .FromSource(command.Source, null)
                .WithOwner(command.CreatedBy)
                .WithPrivacy(command.CreationRequest.CommunityEventPrivacy);
            if (command.CreationRequest.CommunityEventPrivacy.HasValue)
            {
                newEvent.CommunityEventPrivacy = command.CreationRequest.CommunityEventPrivacy.Value;
            }

            await _communityEventRepository.InsertAsync(newEvent);

            await _thunderCqrs.SendEvent(new CommunityEventChangeEvent(newEvent, CommunityEventChangeType.Created), cancellationToken);
        }

        private async Task<bool> HasCreationPermission(Guid communityId, Guid userId)
        {
            var membership = await _communityMembershipRepository
                .FirstOrDefaultAsync(x => x.CommunityId == communityId && x.UserId == userId);

            if (membership == null)
            {
                return false;
            }

            return membership.Role == CommunityMembershipRole.Owner || membership.Role == CommunityMembershipRole.Admin;
        }
    }
}
