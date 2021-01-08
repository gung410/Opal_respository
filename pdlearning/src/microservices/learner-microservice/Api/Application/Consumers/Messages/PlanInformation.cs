using System;

namespace Microservice.Learner.Application.Consumers
{
    public class PlanInformation
    {
        public Guid UserExtId { get; set; }

        public Guid CourseId { get; set; }

        public Guid ResultExtId { get; set; }
    }
}
