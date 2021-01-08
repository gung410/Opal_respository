using System;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Commands
{
    public class DeleteFormCommand : BaseThunderCommand
    {
        public Guid FormId { get; set; }

        public Guid UserId { get; set; }
    }
}
