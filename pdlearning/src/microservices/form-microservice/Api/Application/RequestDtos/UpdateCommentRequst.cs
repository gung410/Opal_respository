using System;

namespace Microservice.Form.Application.RequestDtos
{
    public class UpdateCommentRequest
    {
        public Guid Id { get; set; }

        public string Content { get; set; }
    }
}
