using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class UpdateCommunityCommandHandler : BaseCommandHandler<UpdateCommunityCommand>
    {
        private readonly IRepository<Community> _communityRepository;

        public UpdateCommunityCommandHandler(
            IRepository<Community> communityRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _communityRepository = communityRepository;
        }

        protected override async Task HandleAsync(UpdateCommunityCommand command, CancellationToken cancellationToken)
        {
            var existedCommunity = await _communityRepository.FirstOrDefaultAsync(x => x.Id == command.Id);
            if (existedCommunity == null)
            {
                throw new EntityNotFoundException(typeof(Community), command.Id);
            }

            existedCommunity.Title = command.Title;
            existedCommunity.ParentId = command.ParentId;
            existedCommunity.Status = command.Status;

            await _communityRepository.UpdateAsync(existedCommunity);
        }
    }
}
