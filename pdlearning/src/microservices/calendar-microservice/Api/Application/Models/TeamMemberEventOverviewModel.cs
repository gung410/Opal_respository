using System;

namespace Microservice.Calendar.Application.Models
{
    public class TeamMemberEventOverviewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }
    }
}
