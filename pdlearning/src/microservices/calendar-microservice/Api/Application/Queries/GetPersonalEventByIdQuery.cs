using System;
using Microservice.Calendar.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetPersonalEventByIdQuery : BaseThunderQuery<PersonalEventModel>
    {
        public Guid EventId { get; set; }
    }
}
