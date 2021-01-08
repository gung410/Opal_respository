using System;

namespace Microservice.Badge.Domain.ValueObjects
{
    public class UserStatistic
    {
        public Guid UserId { get; set; }

        public int Count { get; set; }
    }
}
