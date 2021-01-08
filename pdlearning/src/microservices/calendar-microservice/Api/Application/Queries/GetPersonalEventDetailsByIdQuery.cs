using System;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetPersonalEventDetailsByIdQuery : BaseThunderQuery<PersonalEventDetailsModel>
    {
        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public EventType Type { get; set; }
    }
}
