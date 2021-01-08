using System;

namespace Microservice.Calendar.Application.Models
{
    public class TeamMemberEventModel
    {
        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }
    }
}
