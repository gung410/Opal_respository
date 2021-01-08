using System;

namespace Microservice.Learner.Application.Consumers
{
    public class UserMessage
    {
        public int Id { get; set; }

        public Guid Guid { get; set; }
    }
}
