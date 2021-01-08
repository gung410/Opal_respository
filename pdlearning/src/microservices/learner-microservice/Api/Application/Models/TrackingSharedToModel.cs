using System;
using System.Collections.Generic;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class TrackingSharedDetailByModel
    {
        public Guid ItemId { get; set; }

        public LearningTrackingType ItemType { get; set; }

        public string Title { get; set; }

        public List<string> SharedByUsers { get; set; }

        public string ThumbnailUrl { get; set; }
    }
}
