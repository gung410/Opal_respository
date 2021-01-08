using System;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class UpdateCommentRequest
    {
        public Guid Id { get; set; }

        public string Content { get; set; }
    }
}
