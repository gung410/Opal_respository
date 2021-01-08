using System;
using System.Collections.Generic;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetCommunityEventQuery : BaseThunderQuery<List<CommunityEventModel>>
    {
        public GetCommunityEventRequest Request { get; set; }

        public Guid CommunityId { get; set; }
    }
}
