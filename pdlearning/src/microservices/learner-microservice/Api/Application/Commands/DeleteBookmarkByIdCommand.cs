using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class DeleteBookmarkByIdCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }
    }
}
