using System;
using System.Text.Json.Serialization;
using Microservice.Analytics.Domain.ValueObject;

namespace Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads
{
    public class LearnerBookmarkItemPayload
    {
        [JsonPropertyName("isUnbookmark")]
        public bool IsUnBookmark { get; set; }

        public Guid ItemId { get; set; }

        public AnalyticLearnerBookmarkItemType ItemType { get; set; }
    }
}
