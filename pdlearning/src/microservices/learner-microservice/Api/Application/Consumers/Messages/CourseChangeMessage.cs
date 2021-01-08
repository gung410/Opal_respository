using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Consumers
{
    public class CourseChangeMessage : IMQMessageHasCreatedDate
    {
        public Guid Id { get; set; }

        public string CourseName { get; set; }

        public string LearningMode { get; set; }

        public string CourseCode { get; set; }

        public string Description { get; set; }

        public Guid? MOEOfficerId { get; set; }

        public string PDActivityType { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public CourseType CourseType { get; set; }

        public int MaxReLearningTimes { get; set; }

        public Guid? FirstAdministratorId { get; set; }

        public Guid? SecondAdministratorId { get; set; }

        public Guid? PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        public CourseStatus Status { get; set; }

        public ContentStatus ContentStatus { get; set; }

        public DateTime? PublishedContentDate { get; set; }

        public DateTime? SubmittedContentDate { get; set; }

        public string Version { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid ChangedBy { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public DateTime? ApprovalContentDate { get; set; }

        public DateTime? PublishDate { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public string Source { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public int DepartmentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? MessageCreatedDate { get; set; }

        public string ThumbnailUrl { get; set; }
    }
}
