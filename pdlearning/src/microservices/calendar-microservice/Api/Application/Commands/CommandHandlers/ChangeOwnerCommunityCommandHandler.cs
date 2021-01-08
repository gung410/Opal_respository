using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class ChangeOwnerCommunityCommandHandler : BaseCommandHandler<ChangeOwnerCommunityCommand>
    {
        private readonly IRepository<CommunityMembership> _communityMembershipRepository;
        private readonly IRepository<CalendarUser> _userRepository;
        private readonly IRepository<Community> _communityRepository;

        public ChangeOwnerCommunityCommandHandler(
            IRepository<CalendarUser> userRepository,
            IRepository<Community> communityRepository,
            IRepository<CommunityMembership> communityMembershipRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _communityMembershipRepository = communityMembershipRepository;
            _userRepository = userRepository;
            _communityRepository = communityRepository;
        }

        protected override async Task HandleAsync(ChangeOwnerCommunityCommand command, CancellationToken cancellationToken)
        {
            var existedCommunity = await _communityRepository.FirstOrDefaultAsync(x => x.Id == command.CommunityId);
            var existedOwnerCommunity = await _communityMembershipRepository
                .FirstOrDefaultAsync(x => x.CommunityId == command.CommunityId && x.Role == CommunityMembershipRole.Owner);
            if (existedCommunity == null || existedOwnerCommunity == null)
            {
                throw new EntityNotFoundException(typeof(Community), command.CommunityId);
            }

            var newOwnerCommunity = await _userRepository.FirstOrDefaultAsync(x => x.Id == command.NewOwnerId);
            if (newOwnerCommunity == null)
            {
                throw new EntityNotFoundException(typeof(CalendarUser), command.NewOwnerId);
            }

            existedOwnerCommunity.UserId = command.NewOwnerId;
            await _communityMembershipRepository.UpdateAsync(existedOwnerCommunity);
        }
    }
}
