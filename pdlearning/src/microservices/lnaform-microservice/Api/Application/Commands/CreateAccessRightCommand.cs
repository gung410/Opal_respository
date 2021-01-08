using System;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Application.Services;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Commands
{
    public class CreateAccessRightCommand : BaseThunderCommand
    {
        public CreateAccessRightRequest CreationRequest { get; set; }

        public Guid UserId { get; set; }
    }
}
