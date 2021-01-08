using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class DeleteCommunityCommandHandler : BaseCommandHandler<DeleteCommunityCommand>
    {
        private readonly IRepository<Community> _communityRepository;
        private readonly IRepository<CommunityEvent> _communityEventRepository;
        private readonly IRepository<CommunityMembership> _communityMembership;

        public DeleteCommunityCommandHandler(
            IRepository<Community> communityRepository,
            IRepository<CommunityEvent> communityEventRepository,
            IRepository<CommunityMembership> communityMembership,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _communityRepository = communityRepository;
            _communityEventRepository = communityEventRepository;
            _communityMembership = communityMembership;
        }

        protected override async Task HandleAsync(DeleteCommunityCommand command, CancellationToken cancellationToken)
        {
            var existedCommunity = await _communityRepository.FirstOrDefaultAsync(x => x.Id == command.CommunityId);
            if (existedCommunity == null)
            {
                throw new EntityNotFoundException(typeof(Community), command.CommunityId);
            }

            var subCommunityList = await _communityRepository
                .GetAll()
                .Where(x => x.ParentId == command.CommunityId)
                .ToListAsync();
            var subCommunityIds = subCommunityList.Select(p => p.Id).ToList();

            var membershipList = await _communityMembership
                .GetAll()
                .Where(x => x.CommunityId == command.CommunityId || subCommunityIds.Contains(x.CommunityId))
                .ToListAsync();
            await _communityMembership.DeleteManyAsync(membershipList);

            var communityEventList = await _communityEventRepository
                .GetAll()
                .Where(x => x.CommunityId == command.CommunityId || subCommunityIds.Contains(x.CommunityId))
                .ToListAsync();

            await _communityEventRepository.DeleteManyAsync(communityEventList);
            await _communityRepository.DeleteManyAsync(subCommunityList);
            await _communityRepository.DeleteAsync(existedCommunity);
        }
    }
}
