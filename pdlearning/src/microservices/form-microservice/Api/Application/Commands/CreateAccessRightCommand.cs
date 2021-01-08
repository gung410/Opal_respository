using System;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Application.Services;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
{
    public class CreateAccessRightCommand : BaseThunderCommand
    {
        public CreateAccessRightRequest CreationRequest { get; set; }

        public Guid UserId { get; set; }
    }
}
