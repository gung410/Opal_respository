using System;
using Microservice.Content.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Commands
{
    public class SaveVideoCommentCommand : BaseThunderCommand
    {
        public CreateVideoCommentRequest CreationRequest { get; set; }

        public UpdateVideoCommentRequest UpdateRequest { get; set; }

        public Guid UserId { get; set; }

        public bool IsCreation { get; set; }

        public Guid Id { get; set; }
    }
}
