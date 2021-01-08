using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Thunder.Platform.EntityFrameworkCore.Logging;

namespace Microservice.StandaloneSurvey.Application
{
    public class QueryTrackingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IQueryTrackingSource _source;

        public QueryTrackingBehavior(IQueryTrackingSource source)
        {
            _source = source;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var commandName = typeof(TRequest).Name;
            _source.PushTrackingInformation($"Source: {commandName}");
            var response = await next();
            _source.PopTrackingInformation();
            return response;
        }
    }
}
