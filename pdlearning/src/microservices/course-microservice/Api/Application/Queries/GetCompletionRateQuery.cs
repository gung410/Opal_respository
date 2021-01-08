using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetCompletionRateQuery : BaseThunderQuery<double>
    {
        public Guid ClassRunId { get; set; }
    }
}
