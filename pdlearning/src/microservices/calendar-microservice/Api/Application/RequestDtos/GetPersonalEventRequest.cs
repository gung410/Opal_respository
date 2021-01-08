using System;
using Thunder.Platform.Core.Timing;

namespace Microservice.Calendar.Application.RequestDtos
{
    public class GetPersonalEventRequest
    {
        public DateTime OffsetPoint { get; set; } = new DateTime(Clock.Now.Year, 6, 1);

        public int NumberMonthOffset { get; set; } = 7;
    }
}
