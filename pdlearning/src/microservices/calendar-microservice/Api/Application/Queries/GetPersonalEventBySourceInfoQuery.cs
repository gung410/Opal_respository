using System;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetPersonalEventBySourceInfoQuery : BaseThunderQuery<PersonalEventModel>
    {
        public Guid SourceId { get; set; }

        public CalendarEventSource Source { get; set; }
    }
}
