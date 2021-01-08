using System;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Domain.Entities
{
    public class CommunityEvent : EventEntity
    {
        public CommunityEvent(Guid id) : base(id)
        {
        }

        public Guid CommunityId { get; set; }

        public virtual Community Community { get; set; }

        public CommunityEventPrivacy CommunityEventPrivacy { get; set; }
    }
}
