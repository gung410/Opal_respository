using System;

namespace Microservice.Badge.Domain.ValueObjects
{
    /// <summary>
    /// Statistic information which is executed from specific date.
    /// </summary>
    public abstract class BaseStatistic
    {
        public DateTime ExecutedDate { get; init; }
    }
}
