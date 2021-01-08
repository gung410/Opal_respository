using Microservice.Analytics.Domain.ValueObject;

namespace Microservice.Analytics.Application.Consumers.CSL.Messages
{
    public class CSLLikeDeletedMessage
    {
        public int? Id { get; set; }

        public AnalyticCSLLikeSourceType SourceType { get; set; }
    }
}
