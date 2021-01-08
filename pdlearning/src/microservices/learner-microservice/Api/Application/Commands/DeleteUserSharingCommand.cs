using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class DeleteUserSharingCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }
    }
}
