using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class DeleteBookmarkByItemIdCommand : BaseThunderCommand
    {
        public Guid ItemId { get; set; }
    }
}
