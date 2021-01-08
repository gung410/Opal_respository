using System;
using System.Collections.Generic;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class UpdateTrackingRequestDto
    {
        public Guid ItemId { get; set; }

        public LearningTrackingType ItemType { get; set; }

        public bool? IsLike { get; set; }

        public List<Guid> SharedUsers { get; set; }
    }
}
