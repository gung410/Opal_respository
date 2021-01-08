using System;
using System.Collections.Generic;

namespace Microservice.Calendar.Domain.Entities
{
    public class PersonalEvent : EventEntity
    {
        public PersonalEvent() : base()
        {
        }

        public PersonalEvent(Guid id) : base(id)
        {
        }

        public virtual List<UserPersonalEvent> UserEvents { get; set; }
    }
}
