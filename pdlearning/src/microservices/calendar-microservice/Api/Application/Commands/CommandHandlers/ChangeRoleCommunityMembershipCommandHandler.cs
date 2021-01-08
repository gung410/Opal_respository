using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class ChangeRoleCommunityMembershipCommandHandler : BaseCommandHandler<ChangeRoleCommunityMembershipCommand>
    {
        private readonly IRepository<CommunityMembership> _communityMembershipRepository;

        public ChangeRoleCommunityMembershipCommandHandler(
            IRepository<CommunityMembership> communityMembershipRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _communityMembershipRepository = communityMembershipRepository;
        }

        protected override async Task HandleAsync(ChangeRoleCommunityMembershipCommand command, CancellationToken cancellationToken)
        {
            var existedMembership = await _communityMembershipRepository
                .FirstOrDefaultAsync(x => x.CommunityId == command.CommunityId && x.UserId == command.UserId);
            if (existedMembership == null)
            {
                throw new EntityNotFoundException();
            }

            existedMembership.Role = command.Role;
            await _communityMembershipRepository.UpdateAsync(existedMembership);
        }
    }
}
