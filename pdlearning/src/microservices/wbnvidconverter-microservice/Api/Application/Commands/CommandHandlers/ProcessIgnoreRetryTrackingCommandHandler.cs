using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarVideoConverter.Domain.Entities;
using Microservice.WebinarVideoConverter.Domain.Enums;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarVideoConverter.Application.Commands
{
    public class ProcessIgnoreRetryTrackingCommandHandler : BaseCommandHandler<ProcessIgnoreRetryTrackingCommand>
    {
        private readonly IRepository<ConvertingTracking> _convertingTrackingRepo;

        public ProcessIgnoreRetryTrackingCommandHandler(
            IRepository<ConvertingTracking> convertingTrackingRepo,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager)
        {
            _convertingTrackingRepo = convertingTrackingRepo;
        }

        protected override async Task HandleAsync(ProcessIgnoreRetryTrackingCommand command, CancellationToken cancellationToken)
        {
            var tracking = await _convertingTrackingRepo.GetAsync(command.ConvertingTrackingId);
            tracking.Status = ConvertStatus.IgnoreRetry;

            await _convertingTrackingRepo.UpdateAsync(tracking);
        }
    }
}
