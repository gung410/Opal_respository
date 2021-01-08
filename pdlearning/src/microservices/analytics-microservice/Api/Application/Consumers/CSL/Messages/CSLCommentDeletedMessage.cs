using Microservice.Analytics.Domain.ValueObject;

namespace Microservice.Analytics.Application.Consumers.CSL.Messages
{
    public class CSLCommentDeletedMessage
    {
        public int? Id { get; set; }

        public AnalyticCSLCommentThreadType ThreadType { get; set; }
    }
}
