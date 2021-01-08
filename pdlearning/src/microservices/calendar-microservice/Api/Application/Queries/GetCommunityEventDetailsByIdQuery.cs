using System;
using Microservice.Calendar.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetCommunityEventDetailsByIdQuery : BaseThunderQuery<CommunityEventDetailsModel>
    {
        public Guid EventId { get; set; }

        public Guid UserId { get; set; }
    }
}
