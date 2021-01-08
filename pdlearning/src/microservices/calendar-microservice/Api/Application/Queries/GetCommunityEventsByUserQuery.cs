using System;
using System.Collections.Generic;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetCommunityEventsByUserQuery : BaseThunderQuery<List<CommunityEventModel>>
    {
        public Guid UserId { get; set; }

        public GetMyCommunityEventRequest Request { get; set; }
    }
}
