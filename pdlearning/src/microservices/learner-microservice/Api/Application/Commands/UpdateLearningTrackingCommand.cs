using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class UpdateLearningTrackingCommand : BaseThunderCommand
    {
        public Guid ItemId { get; set; }

        public LearningTrackingAction TrackingAction { get; set; }

        public LearningTrackingType TrackingType { get; set; }
    }
}
