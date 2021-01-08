using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarAutoscaler.Application.Commands.CommandHandlers
{
    public class DeleteBBBServerCommandHandler : BaseCommandHandler<DeleteBBBServerCommand>
    {
        private readonly IRepository<BBBServer> _bbbServerRepository;

        public DeleteBBBServerCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<BBBServer> bbbServerRepository) : base(unitOfWorkManager)
        {
            _bbbServerRepository = bbbServerRepository;
        }

        protected override Task HandleAsync(DeleteBBBServerCommand command, CancellationToken cancellationToken)
        {
            return _bbbServerRepository.DeleteAsync(command.BBBServerId);
        }
    }
}
