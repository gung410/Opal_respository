using System;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class CreateCommentRequest : HasSubModuleInfoBase
    {
        public Guid? Id { get; set; }

        public string Content { get; set; }

        public Guid ObjectId { get; set; }
    }
}
