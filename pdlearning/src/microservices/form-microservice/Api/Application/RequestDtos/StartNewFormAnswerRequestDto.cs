using System;

namespace Microservice.Form.Application.RequestDtos
{
    public class SaveFormAnswerRequestDto
    {
        public Guid FormId { get; set; }

        public Guid? CourceId { get; set; }

        public Guid? MyCourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public Guid? AssignmentId { get; set; }
    }
}
