using System;

namespace Microservice.Calendar.Application.Models
{
    public class MemberEventOverviewsModel
    {
        public Guid UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public string FullName
        {
            get
            {
                return ((FirstName ?? string.Empty) + " " + (LastName ?? string.Empty)).Trim();
            }
        }
    }
}
