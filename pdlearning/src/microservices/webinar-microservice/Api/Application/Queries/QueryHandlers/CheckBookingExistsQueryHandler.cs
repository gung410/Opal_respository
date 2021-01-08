using System.Threading;
using System.Threading.Tasks;
using Microservice.Webinar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Webinar.Application.Queries.QueryHandlers
{
    public class CheckBookingExistsQueryHandler : BaseQueryHandler<CheckBookingExistsQuery, bool>
    {
        private readonly IRepository<Booking> _bookingRepository;

        public CheckBookingExistsQueryHandler(IRepository<Booking> bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        protected override Task<bool> HandleAsync(CheckBookingExistsQuery query, CancellationToken cancellationToken)
        {
            return _bookingRepository.GetAll().AnyAsync(x => x.SourceId == query.SessionId && x.Source == query.Source);
        }
    }
}
