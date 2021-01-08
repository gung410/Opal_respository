using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class DeleteUserReviewCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public ItemType ItemType { get; set; }
    }
}
