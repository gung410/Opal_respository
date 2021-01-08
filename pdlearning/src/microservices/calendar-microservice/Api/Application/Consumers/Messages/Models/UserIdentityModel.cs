using System;

namespace Microservice.Calendar.Application.Consumers.Messages.Models
{
    public class UserIdentityModel
    {
        public Guid? ExtId { get; set; }

        public int OwnerId { get; set; }

        public int CustomerId { get; set; }

        public string Archetype { get; set; }

        public int? Id { get; set; }
    }
}
