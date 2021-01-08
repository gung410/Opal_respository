using System;

namespace Microservice.LnaForm.Application.RequestDtos
{
    public class GetFormWithQuestionsByIdRequestDto
    {
        public Guid FormId { get; set; }

        public bool OnlyPublished { get; set; }
    }
}
