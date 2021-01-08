using System;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Domain.ValueObjects;

namespace Microservice.Badge.Domain.Entities
{
    public class BadgeWithCriteria<T> : BadgeEntity where T : BaseBadgeCriteria
    {
        public BadgeWithCriteria(Guid id, BadgeType type) : base(id, type)
        {
        }

        public T Criteria { get; set; }
    }
}
