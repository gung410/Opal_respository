using System;
using Microservice.Content.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Commands
{
    public class UpdateLearningTrackingCommand : BaseThunderCommand
    {
        public Guid UserId { get; set; }

        public Guid ItemId { get; set; }

        public LearningTrackingAction TrackingAction { get; set; }

        public LearningTrackingType TrackingType { get; set; }
    }
}
