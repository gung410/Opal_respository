using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class ArchiveCommunityCommandHandler : BaseCommandHandler<ArchiveCommunityCommand>
    {
        private readonly IRepository<Community> _communityRepo;

        public ArchiveCommunityCommandHandler(
            IRepository<Community> communityRepo,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _communityRepo = communityRepo;
        }

        protected override async Task HandleAsync(ArchiveCommunityCommand command, CancellationToken cancellationToken)
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
