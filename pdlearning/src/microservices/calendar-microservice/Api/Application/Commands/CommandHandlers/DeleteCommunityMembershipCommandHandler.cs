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
    public class DeleteCommunityMembershipCommandHandler : BaseCommandHandler<DeleteCommunityMembershipCommand>
    {
        private readonly IRepository<CommunityMembership> _communityMembershipRepository;

        public DeleteCommunityMembershipCommandHandler(
            IRepository<CommunityMembership> communityMembershipRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _communityMembershipRepository = communityMembershipRepository;
        }

        protected override async Task HandleAsync(DeleteCommunityMembershipCommand command, CancellationToken cancellationToken)
        {
            var existedMembership = await _communityMembershipRepository
                .FirstOrDefaultAsync(x => x.CommunityId == command.CommunityId && x.UserId == command.UserId && x.Role != CommunityMembershipRole.Owner);
            if (existedMembership == null)
            {
                throw new EntityNotFoundException();
            }

            await _communityMembershipRepository.DeleteAsync(existedMembership);
        }
    }
}
