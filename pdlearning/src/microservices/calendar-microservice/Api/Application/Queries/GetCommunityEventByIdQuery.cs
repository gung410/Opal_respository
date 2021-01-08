using System;
using Microservice.Calendar.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetCommunityEventByIdQuery : BaseThunderQuery<CommunityEventModel>
    {
        public Guid EventId { get; set; }
    }
}
