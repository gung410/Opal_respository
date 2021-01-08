using System;

namespace Microservice.Calendar.Application.RequestDtos
{
    public class GetPersonalEventByRangeRequest
    {
        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }
    }
}
