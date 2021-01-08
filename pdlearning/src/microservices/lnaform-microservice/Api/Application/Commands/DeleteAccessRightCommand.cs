using System;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Commands
{
    public class DeleteAccessRightCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }
    }
}
