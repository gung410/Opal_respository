using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.SharedQueries;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetPersonalEventByRangeQueryHandler : BaseThunderQueryHandler<GetPersonalEventByRangeQuery, List<EventModel>>
    {
        private readonly PersonalEventSharedQuery _personalEventSharedQuery;

        public GetPersonalEventByRangeQueryHandler(PersonalEventSharedQuery personalEventSharedQuery)
        {
            _personalEventSharedQuery = personalEventSharedQuery;
        }

        protected override Task<List<EventModel>> HandleAsync(GetPersonalEventByRangeQuery query, CancellationToken cancellationToken)
        {
            return _personalEventSharedQuery.GetPersonalEventByRange(query.UserId, query.Request.StartAt, query.Request.EndAt, cancellationToken);
        }
    }
}
