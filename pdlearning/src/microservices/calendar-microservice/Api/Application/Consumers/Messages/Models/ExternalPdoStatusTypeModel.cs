using Microservice.Calendar.Application.Consumers.Messages.Enums;

namespace Microservice.Calendar.Application.Consumers.Messages.Models
{
    public class ExternalPdoStatusTypeModel
    {
        public int StatusTypeId { get; set; }

        public AssessmentStatus StatusTypeCode { get; set; }
    }
}
