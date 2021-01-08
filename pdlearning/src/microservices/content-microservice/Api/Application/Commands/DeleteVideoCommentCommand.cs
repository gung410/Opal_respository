using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Commands
{
    public class DeleteVideoCommentCommand : BaseThunderCommand
    {
        public Guid UserId { get; set; }

        public Guid Id { get; set; }
    }
}
