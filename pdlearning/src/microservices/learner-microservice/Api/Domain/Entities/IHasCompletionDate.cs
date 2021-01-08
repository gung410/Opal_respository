using System;

namespace Microservice.Learner.Domain.Entities
{
    public interface IHasCompletionDate
    {
        DateTime? CompletedDate { get; set; }
    }
}
