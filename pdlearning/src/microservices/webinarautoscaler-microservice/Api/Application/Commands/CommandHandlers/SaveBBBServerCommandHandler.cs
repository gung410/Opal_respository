using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarAutoscaler.Application.Commands.CommandHandlers
{
    public class SaveBBBServerCommandHandler : BaseCommandHandler<SaveBBBServerCommand>
    {
        private readonly IRepository<BBBServer> _bbbServerRepository;

        public SaveBBBServerCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<BBBServer> bbbServerRepository) : base(unitOfWorkManager)
        {
            _bbbServerRepository = bbbServerRepository;
        }

        protected override Task HandleAsync(SaveBBBServerCommand command, CancellationToken cancellationToken)
        {
            var bbbServer = new BBBServer()
            {
                InstanceId = command.InstanceId,
                RuleArn = command.RuleArn,
                TargetGroupArn = command.TargetGroupArn,
                PrivateIp = command.PrivateIp,
                Status = command.Status,
                IsProtection = command.IsProtection
            };
            return _bbbServerRepository.InsertAsync(bbbServer);
        }
    }
}
