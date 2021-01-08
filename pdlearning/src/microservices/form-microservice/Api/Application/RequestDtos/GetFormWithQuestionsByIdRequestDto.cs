using System;

namespace Microservice.Form.Application.RequestDtos
{
    public class GetFormWithQuestionsByIdRequestDto
    {
        public Guid FormId { get; set; }

        public bool OnlyPublished { get; set; }
    }
}
