using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
{
    public class MarkFormAsArchivedCommand : BaseThunderCommand
    {
        public Guid UserId { get; set; }

        public Guid Id { get; set; }
    }
}
