using System;

namespace Microservice.Learner.Application.Models
{
    public interface IHasStatistic<out TEnum> where TEnum : Enum
    {
        public TEnum Type { get; }

        public int TotalCount { get; }
    }
}
