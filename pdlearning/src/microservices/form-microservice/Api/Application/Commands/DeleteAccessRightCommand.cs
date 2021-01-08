using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
{
    public class DeleteAccessRightCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }
    }
}
