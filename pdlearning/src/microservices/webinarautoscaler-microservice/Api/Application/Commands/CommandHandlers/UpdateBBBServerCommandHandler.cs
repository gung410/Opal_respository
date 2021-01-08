using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarAutoscaler.Application.Commands.CommandHandlers
{
    public class UpdateBBBServerCommandHandler : BaseCommandHandler<UpdateBBBServerCommand>
    {
        private readonly IRepository<BBBServer> _bbbServerRepository;

        public UpdateBBBServerCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<BBBServer> bbbServerRepository) : base(unitOfWorkManager)
        {
            _bbbServerRepository = bbbServerRepository;
        }

        protected override async Task HandleAsync(UpdateBBBServerCommand command, CancellationToken cancellationToken)
        {
            if (command.BBBServer == null)
            {
                return;
            }

            var bbb = await _bbbServerRepository.GetAsync(command.BBBServer.Id);

            if (bbb == null)
            {
                throw new EntityNotFoundException($"BBB Server id: {command.BBBServer.Id} not found in db.");
            }

            bbb.PrivateIp = command.BBBServer.PrivateIp;
            bbb.Status = command.BBBServer.Status;
            bbb.IsProtection = command.BBBServer.IsProtection;
            bbb.InstanceId = command.BBBServer.InstanceId;
            bbb.RuleArn = command.BBBServer.RuleArn;
            bbb.TargetGroupArn = command.BBBServer.TargetGroupArn;

            await _bbbServerRepository.UpdateAsync(bbb);
        }
    }
}
