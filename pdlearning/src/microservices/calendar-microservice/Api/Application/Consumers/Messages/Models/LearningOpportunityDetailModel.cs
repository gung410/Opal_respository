using System;

namespace Microservice.Calendar.Application.Consumers.Messages.Models
{
    public class LearningOpportunityDetailModel
    {
        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
