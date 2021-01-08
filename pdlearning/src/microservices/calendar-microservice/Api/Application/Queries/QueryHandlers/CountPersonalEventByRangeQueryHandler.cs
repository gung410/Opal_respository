using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.SharedQueries;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries.QueryHandlers
{
    public class CountPersonalEventByRangeQueryHandler : BaseThunderQueryHandler<CountPersonalEventByRangeQuery, int>
    {
        private readonly PersonalEventSharedQuery _personalEventSharedQuery;

        public CountPersonalEventByRangeQueryHandler(PersonalEventSharedQuery personalEventSharedQuery)
        {
            _personalEventSharedQuery = personalEventSharedQuery;
        }

        protected override Task<int> HandleAsync(CountPersonalEventByRangeQuery query, CancellationToken cancellationToken)
        {
            return _personalEventSharedQuery.CountPersonalEventByRange(query.UserId, query.Request.StartAt, query.Request.EndAt, cancellationToken);
        }
    }
}
