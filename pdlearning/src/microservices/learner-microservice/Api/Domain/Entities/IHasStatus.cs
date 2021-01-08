using System;

namespace Microservice.Learner.Domain.Entities
{
    public interface IHasStatus<TEnum> where TEnum : Enum
    {
        TEnum Status { get; set; }
    }
}
