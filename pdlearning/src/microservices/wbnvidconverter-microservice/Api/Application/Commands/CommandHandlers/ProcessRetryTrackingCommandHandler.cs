using System;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarVideoConverter.Domain.Constants;
using Microservice.WebinarVideoConverter.Domain.Entities;
using Microservice.WebinarVideoConverter.Domain.Enums;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarVideoConverter.Application.Commands.CommandHandlers
{
    public class ProcessRetryTrackingCommandHandler : BaseCommandHandler<ProcessRetryTrackingCommand>
    {
        private readonly IRepository<ConvertingTracking> _convertingTrackingRepo;

        public ProcessRetryTrackingCommandHandler(
            IRepository<ConvertingTracking> convertingTrackingRepo,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager)
        {
            _convertingTrackingRepo = convertingTrackingRepo;
        }

        protected override async Task HandleAsync(ProcessRetryTrackingCommand command, CancellationToken cancellationToken)
        {
            var convertingTracking = await _convertingTrackingRepo.GetAsync(command.ConvertingTrackingId);

            var status = GetPreviousStep(convertingTracking.FailedAtStep);
            convertingTracking.Status = status;
            convertingTracking.RetryCount += 1;

            await _convertingTrackingRepo.UpdateAsync(convertingTracking);
        }

        private ConvertStatus GetPreviousStep(FailStep failStep)
        {
            var mappings = FailedRecordHandlingServiceConstants._failStepMapping;
            if (!mappings.ContainsKey(failStep))
            {
                throw new InvalidOperationException($"Failed to get new status from FailStep, mapping does not exist. FailStep: {failStep}");
            }

            return mappings[failStep];
        }
    }
}
