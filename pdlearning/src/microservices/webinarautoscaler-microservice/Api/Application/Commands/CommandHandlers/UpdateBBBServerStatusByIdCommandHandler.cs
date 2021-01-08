using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarAutoscaler.Application.Commands.CommandHandlers
{
    public class UpdateBBBServerStatusByIdCommandHandler : BaseCommandHandler<UpdateBBBServerStatusByIdCommand>
    {
        private readonly IRepository<BBBServer> _bbbServerRepository;

        public UpdateBBBServerStatusByIdCommandHandler(
            IRepository<BBBServer> bbbServerRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager)
        {
            _bbbServerRepository = bbbServerRepository;
        }

        protected override async Task HandleAsync(UpdateBBBServerStatusByIdCommand command, CancellationToken cancellationToken)
        {
            var bbb = await _bbbServerRepository
                .GetAll()
                .FirstOrDefaultAsync(p => p.Id == command.BBBServerId, cancellationToken);

            if (bbb == null)
            {
                throw new EntityNotFoundException($"BBB Server id: {command.BBBServerId} not found in db.");
            }

            bbb.Status = command.Status;

            await _bbbServerRepository.UpdateAsync(bbb);
        }
    }
}
