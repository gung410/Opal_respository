namespace Microservice.Calendar.Application.Consumers.Messages.Models
{
    public class LearningOpportunityModel
    {
        public string Name { get; set; }

        public LearningOpportunityDetailModel Extensions { get; set; }

        public string Source { get; set; }
    }
}
