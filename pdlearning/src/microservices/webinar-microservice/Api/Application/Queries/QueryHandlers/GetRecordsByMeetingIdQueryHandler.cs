using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Webinar.Application.Models;
using Microservice.Webinar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Queries.QueryHandlers
{
    public class GetRecordsByMeetingIdQueryHandler : BaseThunderQueryHandler<GetRecordsByBookingSourceIdQuery, List<RecordModel>>
    {
        private readonly IRepository<Booking> _bookingRepository;
        private readonly IRepository<Record> _recordRepository;

        public GetRecordsByMeetingIdQueryHandler(
            IRepository<Booking> bookingRepository,
            IRepository<Record> recordRepository)
        {
            _bookingRepository = bookingRepository;
            _recordRepository = recordRepository;
        }

        protected override async Task<List<RecordModel>> HandleAsync(GetRecordsByBookingSourceIdQuery query, CancellationToken cancellationToken)
        {
            var bookingInfo = await _bookingRepository.FirstOrDefaultAsync(p => p.SourceId == query.SourceId);

            if (bookingInfo == null)
            {
                return new List<RecordModel>();
            }

            return await _recordRepository.GetAll()
                .Where(p => p.MeetingId == bookingInfo.MeetingId)
                .Select(r => new RecordModel
                {
                    Id = r.Id,
                    Status = r.Status,
                    MeetingId = r.MeetingId,
                    RecordId = r.RecordId,
                    DigitalContentId = r.DigitalContentId
                })
                .ToListAsync(cancellationToken);
        }
    }
}
