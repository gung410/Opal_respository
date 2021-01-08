using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Queries
{
    public class GetBBBServerIdsForScalingInQuery : BaseThunderQuery<List<Guid>>
    {
    }
}
