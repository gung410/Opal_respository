using System;

namespace Microservice.Form.Application.RequestDtos
{
    public class CloneFormRequestDto
    {
        public Guid FormId { get; set; }

        public string NewTitle { get; set; }
    }
}
