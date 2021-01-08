using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class CreateCommunityMembershipCommandHandler : BaseCommandHandler<CreateCommunityMembershipCommand>
    {
        private readonly IRepository<CommunityMembership> _communityMembershipRepository;

        public CreateCommunityMembershipCommandHandler(
            IRepository<CommunityMembership> communityMembershipRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _communityMembershipRepository = communityMembershipRepository;
        }

        protected override async Task HandleAsync(CreateCommunityMembershipCommand command, CancellationToken cancellationToken)
        {
            var newMembership = new CommunityMembership
            {
                CommunityId = command.CommunityId,
                UserId = command.UserId,
                Role = command.Role
            };
            await _communityMembershipRepository.InsertAsync(newMembership);
        }
    }
}
