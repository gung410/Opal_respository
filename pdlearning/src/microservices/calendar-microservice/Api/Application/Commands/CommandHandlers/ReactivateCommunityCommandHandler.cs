using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class ReactivateCommunityCommandHandler : BaseCommandHandler<ReactivateCommunityCommand>
    {
        private readonly IRepository<Community> _communityRepo;
        private readonly IThunderCqrs _thunderCqrs;

        public ReactivateCommunityCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IRepository<Community> communityRepo,
            IThunderCqrs thunderCqrs) : base(unitOfWorkManager, userContext)
        {
            _communityRepo = communityRepo;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(ReactivateCommunityCommand command, CancellationToken cancellationToken)
        {
            var community = await _communityRepo.FirstOrDefaultAsync(c => c.Id == command.Id);
            if (community == null)
            {
                throw new EntityNotFoundException(typeof(Community), community.Id);
            }

            community.Status = command.Status;

            _communityRepo.Update(community);
        }
    }
}
