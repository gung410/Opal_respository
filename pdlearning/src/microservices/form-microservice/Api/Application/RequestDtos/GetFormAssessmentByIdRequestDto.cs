using System;

namespace Microservice.Form.Application.RequestDtos
{
    public class GetFormAssessmentByIdRequestDto
    {
        public Guid FormId { get; set; }

        public bool OnlyPublished { get; set; }
    }
}
