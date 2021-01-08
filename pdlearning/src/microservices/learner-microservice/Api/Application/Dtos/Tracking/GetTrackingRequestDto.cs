using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class GetTrackingByItemIdRequestDto
    {
        public Guid ItemId { get; set; }

        public LearningTrackingType ItemType { get; set; }
    }
}
