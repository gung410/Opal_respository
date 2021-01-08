using System;

namespace Microservice.LnaForm.Application.RequestDtos
{
    public class CloneFormRequestDto
    {
        public Guid FormId { get; set; }

        public string NewTitle { get; set; }
    }
}
