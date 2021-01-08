using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Webinar.Application.Exception;
using Microservice.Webinar.Application.Models;
using Microservice.Webinar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Queries.QueryHandlers
{
    public class GetMeetingBySourceQueryHandler : BaseThunderQueryHandler<GetMeetingBySourceQuery, MeetingInfoModel>
    {
        private readonly IRepository<Booking> _bookingRepository;
        private readonly IRepository<MeetingInfo> _meetingInfoRepository;

        public GetMeetingBySourceQueryHandler(
            IRepository<Booking> bookingRepository,
            IRepository<MeetingInfo> meetingInfoRepository)
        {
            _bookingRepository = bookingRepository;
            _meetingInfoRepository = meetingInfoRepository;
        }

        protected override async Task<MeetingInfoModel> HandleAsync(GetMeetingBySourceQuery query, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.FirstOrDefaultAsync(x => x.SourceId == query.SourceId && x.Source == query.Source);

            if (booking == null)
            {
                throw new BookingNotFoundException();
            }

            return await _meetingInfoRepository.GetAll()
                .Where(x => x.Id == booking.MeetingId)
                .Select(p => new MeetingInfoModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    StartTime = p.StartTime,
                    EndTime = p.EndTime,
                    BBBServerPrivateIp = p.BBBServerPrivateIp,
                    IsCanceled = p.IsCanceled
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
