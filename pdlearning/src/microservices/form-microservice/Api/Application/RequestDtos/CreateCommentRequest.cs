using System;

namespace Microservice.Form.Application.RequestDtos
{
    public class CreateCommentRequest
    {
        public Guid? Id { get; set; }

        public string Content { get; set; }

        public Guid ObjectId { get; set; }
    }
}
