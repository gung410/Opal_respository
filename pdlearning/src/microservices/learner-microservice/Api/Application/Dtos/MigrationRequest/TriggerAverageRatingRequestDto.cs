using System;
using System.Collections.Generic;

namespace Microservice.Learner.Application.Dtos
{
    public class TriggerAverageRatingRequestDto
    {
        public List<Guid> DigitalContentIds { get; set; }

        public int BatchSize { get; set; }
    }
}
