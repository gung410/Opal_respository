using System;
using System.Collections.Generic;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetPersonalEventByRangeQuery : BaseThunderQuery<List<EventModel>>
    {
        public GetPersonalEventByRangeRequest Request { get; set; }

        public Guid UserId { get; set; }
    }
}
