using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class TrackingModel
    {
        public Guid ItemId { get; set; }

        public LearningTrackingType ItemType { get; set; }

        public bool IsLike { get; set; }

        public int TotalLike { get; set; }

        public int TotalShare { get; set; }

        public int TotalView { get; set; }

        public int TotalDownload { get; set; }
    }
}
