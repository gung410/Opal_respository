using System;

namespace Microservice.Calendar.Application.Models
{
    public class SharedTeamModel
    {
        public Guid AccessShareId { get; set; }

        public string OwnerFullName { get; set; }
    }
}
