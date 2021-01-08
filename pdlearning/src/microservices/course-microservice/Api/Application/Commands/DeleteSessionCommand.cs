using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class DeleteSessionCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }
    }
}
