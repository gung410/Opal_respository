using System;
using Microservice.Badge.Domain.Enums;

namespace Microservice.Badge.Domain.ValueObjects
{
    public class BadgeLevel
    {
        public BadgeLevelEnum Level { get; init; }

        public DateTime? IncreaseLevelDate { get; init; }
    }
}
