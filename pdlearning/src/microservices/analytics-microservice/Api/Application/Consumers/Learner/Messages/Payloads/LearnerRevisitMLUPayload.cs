using System;
using Microservice.Analytics.Domain.ValueObject;

namespace Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads
{
    public class LearnerRevisitMLUPayload
    {
        public Guid CourseId { get; set; }

        public AnalyticLearnerRevisitMLUMode VisitMode { get; set; }
    }
}
