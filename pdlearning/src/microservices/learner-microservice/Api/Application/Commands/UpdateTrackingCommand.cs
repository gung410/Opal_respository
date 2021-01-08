using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class UpdateTrackingCommand : BaseThunderCommand
    {
        public Guid ItemId { get; set; }

        public LearningTrackingType ItemType { get; set; }

        public LearningTrackingAction TrackingAction { get; set; } = LearningTrackingAction.Like;

        public bool IsLike { get; set; }
    }
}
