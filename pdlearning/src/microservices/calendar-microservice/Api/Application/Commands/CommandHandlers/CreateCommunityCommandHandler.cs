using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class CreateCommunityCommandHandler : BaseCommandHandler<CreateCommunityCommand>
    {
        private readonly IRepository<Community> _communityRepository;
        private readonly IRepository<CommunityMembership> _communityMembershipRepository;

        public CreateCommunityCommandHandler(
            IRepository<CommunityMembership> communityMembershipRepository,
            IRepository<Community> communityRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _communityRepository = communityRepository;
            _communityMembershipRepository = communityMembershipRepository;
        }

        protected override async Task HandleAsync(CreateCommunityCommand command, CancellationToken cancellationToken)
        {
            var newCommunity = new Community
            {
                Id = command.Id,
                Title = command.Title,
                ParentId = command.ParentId,
                Status = command.Status
            };
            await _communityRepository.InsertAsync(newCommunity);

            var ownerCommunity = new CommunityMembership
            {
                CommunityId = command.Id,
                UserId = command.OwnerId,
                Role = CommunityMembershipRole.Owner
            };
            await _communityMembershipRepository.InsertAsync(ownerCommunity);
        }
    }
}
