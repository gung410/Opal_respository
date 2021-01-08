using Microservice.Course.Attributes;

namespace Microservice.Course.Application.RequestDtos
{
    public class ImportPartcipantDto
    {
        [TableColumn(0, "classrun_code")]
        public string ClassRunCode { get; set; }

        [TableColumn(1, "learner_email")]
        public string LearnerEmail { get; set; }
    }
}
