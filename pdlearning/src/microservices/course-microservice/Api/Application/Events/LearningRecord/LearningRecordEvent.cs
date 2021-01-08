using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Course.Application.Enums;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class LearningRecordEvent : BaseThunderEvent, IMQMessage
    {
        public LearningRecordEvent()
        {
        }

        public LearningRecordEvent(Registration registration, CourseEntity course, ClassRun classRun, Guid correlationId)
        {
            var recordType = course.IsMicroLearning() ? RecordType.MicroLearning : RecordType.Course;
            CorrelationId = correlationId;
            Status = registration.LearningStatus;
            UserId = registration.UserId;
            CourseId = registration.CourseId;
            Year = registration.CreatedDate.Year;
            ClassRunId = registration.ClassRunId;
            RecordType = recordType;
            CourseFee = course.CourseFee;
            CourseCode = course.CourseCode;
            CourseType = course.CourseType;
            CourseTitle = course.CourseName;
            EndDate = classRun.EndDateTime;
            StartDate = classRun.StartDateTime;
            DurationHours = course.DurationHours;
            ClassRunTitle = classRun.ClassTitle;
            CourseDescription = course.Description;
            ModeOfLearningId = course.LearningMode;
            ClassRunCode = classRun.ClassRunCode;
            MainSubjectAreaId = course.PDAreaThemeId;
            DurationMinutes = course.DurationMinutes;
            RecordUri = $"CAM:{registration.CreatedDate.Year}:{recordType}:{registration.CourseId}:user:{registration.UserId}";
        }

        public string RecordUri { get; set; }

        public RecordType RecordType { get; set; }

        public int Year { get; set; }

        public Guid UserId { get; set; }

        public CourseType CourseType { get; set; }

        public Guid CourseId { get; set; }

        public string CourseCode { get; set; }

        public string CourseTitle { get; set; }

        public string CourseDescription { get; set; }

        public decimal? CourseFee { get; set; }

        public string MainSubjectAreaId { get; set; }

        public string ModeOfLearningId { get; set; }

        public Guid ClassRunId { get; set; }

        public string ClassRunCode { get; set; }

        public string ClassRunTitle { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? DurationHours { get; set; }

        public int? DurationMinutes { get; set; }

        public Guid CorrelationId { get; set; }

        public LearningStatus Status { get; set; }
    }
}
