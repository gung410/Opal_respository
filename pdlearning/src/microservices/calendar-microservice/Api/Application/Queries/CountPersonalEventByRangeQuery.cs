using System;
using Microservice.Calendar.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class CountPersonalEventByRangeQuery : BaseThunderQuery<int>
    {
        public GetPersonalEventByRangeRequest Request { get; set; }

        public Guid UserId { get; set; }
    }
}
