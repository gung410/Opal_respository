using Thunder.Platform.Cqrs;

namespace Microservice.Analytics.Application.Queries.QueryHandlers
{
    public abstract class BaseQueryHandler<TQuery, TResult> : BaseThunderQueryHandler<TQuery, TResult> where TQuery : BaseThunderQuery<TResult>
    {
    }
}
