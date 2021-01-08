using Microservice.Calendar.Application.Consumers.Messages.Enums;

namespace Microservice.Calendar.Application.Consumers.Messages.Models
{
    public class AssessmentStatusInfoModel
    {
        public int AssessmentStatusId { get; set; }

        public AssessmentStatus AssessmentStatusCode { get; set; }
    }
}
