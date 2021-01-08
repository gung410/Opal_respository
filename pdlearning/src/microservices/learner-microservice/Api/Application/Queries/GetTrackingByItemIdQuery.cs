using System;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetTrackingByItemIdQuery : BaseThunderQuery<TrackingModel>
    {
        public Guid ItemId { get; set; }

        public LearningTrackingType ItemType { get; set; }

        public LearningTrackingAction TrackingAction { get; set; } = LearningTrackingAction.All;
    }
}
