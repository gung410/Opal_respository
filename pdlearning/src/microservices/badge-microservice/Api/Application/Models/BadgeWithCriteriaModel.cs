using Microservice.Badge.Domain.ValueObjects;

namespace Microservice.Badge.Application.Models
{
    public class BadgeWithCriteriaModel<T> : BadgeModel where T : BaseBadgeCriteria
    {
        public T Criteria { get; set; }
    }
}
