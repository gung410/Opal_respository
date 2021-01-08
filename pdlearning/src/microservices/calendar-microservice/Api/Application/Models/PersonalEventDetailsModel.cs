using System;
using System.Collections.Generic;

namespace Microservice.Calendar.Application.Models
{
    public class PersonalEventDetailsModel : PersonalEventModel
    {
        public IEnumerable<Guid> AttendeeIds { get; set; }
    }
}
