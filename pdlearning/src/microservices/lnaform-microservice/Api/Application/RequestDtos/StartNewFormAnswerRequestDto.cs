using System;

namespace Microservice.LnaForm.Application.RequestDtos
{
    public class SaveFormAnswerRequestDto
    {
        public Guid FormId { get; set; }

        public Guid? ResourceId { get; set; }
    }
}
