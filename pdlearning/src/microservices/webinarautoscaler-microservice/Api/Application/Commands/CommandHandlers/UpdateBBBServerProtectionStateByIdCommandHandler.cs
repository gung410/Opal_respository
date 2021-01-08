using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarAutoscaler.Application.Commands.CommandHandlers
{
    public class UpdateBBBServerProtectionStateByIdCommandHandler : BaseCommandHandler<UpdateBBBServerProtectionStateByIdCommand>
    {
        private readonly IRepository<BBBServer> _bbbServerRepository;

        public UpdateBBBServerProtectionStateByIdCommandHandler(
            IRepository<BBBServer> bbbServerRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager)
        {
            _bbbServerRepository = bbbServerRepository;
        }

        protected override async Task HandleAsync(UpdateBBBServerProtectionStateByIdCommand command, CancellationToken cancellationToken)
        {
            var bbb = await _bbbServerRepository
                .GetAll()
                .FirstOrDefaultAsync(p => p.Id == command.BBBServerId, cancellationToken);

            if (bbb == null)
            {
                throw new EntityNotFoundException($"BBB Server id: {command.BBBServerId} not found in db.");
            }

            bbb.IsProtection = command.IsProtection;

            await _bbbServerRepository.UpdateAsync(bbb);
        }
    }
}
