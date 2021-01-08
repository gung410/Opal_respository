using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarVideoConverter.Application.Events;
using Microservice.WebinarVideoConverter.Application.RequestDtos;
using Microservice.WebinarVideoConverter.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarVideoConverter.Application.Commands
{
    public class SaveConvertingTrackingsCommandHandler : BaseCommandHandler<SaveConvertingTrackingsCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<ConvertingTracking> _convertingTrackingRepository;

        public SaveConvertingTrackingsCommandHandler(
            IThunderCqrs thunderCqrs,
            IRepository<ConvertingTracking> convertingTrackingRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager)
        {
            _thunderCqrs = thunderCqrs;
            _convertingTrackingRepository = convertingTrackingRepository;
        }

        protected override async Task HandleAsync(SaveConvertingTrackingsCommand command, CancellationToken cancellationToken)
        {
            var internalMeetingIds = command
                .ConvertTrackings
                .Select(tracking => tracking.InternalMeetingId)
                .ToList();

            var existingTrackings = _convertingTrackingRepository
                .GetAll()
                .Where(tracking => internalMeetingIds.Contains(tracking.InternalMeetingId))
                .ToList();

            var existingTrackingIds = existingTrackings
                .Select(eTracking => eTracking.InternalMeetingId)
                .ToList();

            var newTrackings = command
                .ConvertTrackings
                .Where(tracking => !existingTrackingIds.Contains(tracking.InternalMeetingId))
                .ToList();

            await _convertingTrackingRepository.InsertManyAsync(newTrackings);

            var events = newTrackings.Select(tracking =>
                new WebinarRecordChangeEvent(new WebinarRecordChangeRequest
                {
                    MeetingId = tracking.MeetingId,
                    RecordId = tracking.Id,
                    InternalMeetingId = tracking.InternalMeetingId,
                    Status = tracking.Status
                }));

            await _thunderCqrs.SendEvents(events, cancellationToken);
        }
    }
}
