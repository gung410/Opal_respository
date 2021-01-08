using System;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class SaveCommentCommand : BaseStandaloneSurveyCommand
    {
        public CreateCommentRequest CreationRequest { get; set; }

        public UpdateCommentRequest UpdateRequest { get; set; }

        public Guid UserId { get; set; }

        public bool IsCreation { get; set; }

        public Guid Id { get; set; }
    }
}
