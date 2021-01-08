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
    public class GetAttendeeQueryHandler : BaseThunderQueryHandler<GetAttendeeQuery, AttendeeModel>
    {
        private readonly IRepository<Attendee> _attendeeRepository;

        public GetAttendeeQueryHandler(IRepository<Attendee> attendeeRepository)
        {
            _attendeeRepository = attendeeRepository;
        }

        protected override Task<AttendeeModel> HandleAsync(GetAttendeeQuery query, CancellationToken cancellationToken)
        {
            return _attendeeRepository.GetAll()
                .Where(x => x.MeetingId == query.MeetingId && x.UserId == query.UserId)
                .Select(p => new AttendeeModel
                {
                    IsModerator = p.IsModerator,
                    MeetingId = p.MeetingId,
                    UserId = p.UserId
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
