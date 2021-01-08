using System;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Application.Services;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Commands
{
    public class SaveCommentCommand : BaseThunderCommand
    {
        public CreateCommentRequest CreationRequest { get; set; }

        public UpdateCommentRequest UpdateRequest { get; set; }

        public Guid UserId { get; set; }

        public bool IsCreation { get; set; }

        public Guid Id { get; set; }
    }
}
