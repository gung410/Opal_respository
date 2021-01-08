using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using JetBrains.Annotations;
using Microservice.Learner.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Events
{
    public class LearningRecordEvent : BaseThunderEvent, IMQMessage
    {
        public LearningRecordEvent(MyCourse myCourse, [CanBeNull] UserReview userReview)
        {
            CourseId = myCourse.CourseId;
            UserId = myCourse.UserId;
            RecordType = myCourse.CourseType.ToString();
            Year = myCourse.CreatedDate.Year;
            StartDate = myCourse.StartDate;
            EndDate = myCourse.EndDate;
            Status = myCourse.Status.ToString();
            CreatedBy = myCourse.UserId;
            Rating = userReview != null ? userReview.Rate : null;
            Review = userReview != null ? userReview.CommentContent : null;
            RecordUri = $"learner:{myCourse.CreatedDate.Year}:{myCourse.CourseType.ToString().ToLower()}:{myCourse.CourseId}:user:{myCourse.UserId}";
        }

        public string RecordUri { get; set; }

        public string RecordType { get; set; }

        public int Year { get; set; }

        public Guid UserId { get; set; }

        public Guid CourseId { get; set; }

        public double? Rating { get; set; }

        public string Review { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Status { get; set; }

        public Guid CreatedBy { get; set; }

        public override string GetRoutingKey()
        {
            return "learningrecord.post";
        }
    }
}
