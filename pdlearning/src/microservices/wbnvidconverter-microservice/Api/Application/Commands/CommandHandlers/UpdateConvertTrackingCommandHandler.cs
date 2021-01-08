using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarVideoConverter.Domain.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarVideoConverter.Application.Commands
{
    public class UpdateConvertTrackingCommandHandler : BaseCommandHandler<UpdateConvertTrackingCommand>
    {
        private readonly IRepository<ConvertingTracking> _convertingTrackingRepository;

        public UpdateConvertTrackingCommandHandler(
           IRepository<ConvertingTracking> convertingTrackingRepository,
           IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager)
        {
            _convertingTrackingRepository = convertingTrackingRepository;
        }

        protected override async Task HandleAsync(UpdateConvertTrackingCommand command, CancellationToken cancellationToken)
        {
            var existedTracking = await _convertingTrackingRepository.FirstOrDefaultAsync(x => x.Id == command.Id);
            if (existedTracking == null)
            {
                throw new EntityNotFoundException(typeof(ConvertingTracking), command.Id);
            }

            foreach (var propToUpdate in command.Properties)
            {
                var property = existedTracking.GetType().GetProperty(propToUpdate.Key);
                property.SetValue(existedTracking, propToUpdate.Value);
            }

            await _convertingTrackingRepository.UpdateAsync(existedTracking);
        }
    }
}
