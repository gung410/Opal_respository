using System;
using System.Collections.Generic;

namespace Microservice.Learner.Application.Dtos
{
    public class MigrateAverageRatingRequestDto
    {
        public List<Guid> DigitalContentIds { get; set; }

        public int BatchSize { get; set; }
    }
}
