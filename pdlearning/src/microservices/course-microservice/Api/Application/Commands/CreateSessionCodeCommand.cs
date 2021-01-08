using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class CreateSessionCodeCommand : BaseThunderCommand
    {
        public Guid SessionId { get; set; }
    }
}
