using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Events
{
    public class MyDigitalContentRecordEvent : BaseThunderEvent, IMQMessage
    {
        public MyDigitalContentRecordEvent(MyDigitalContent myDigitalContent)
        {
            RecordType = "DigitalContent";
            Year = myDigitalContent.CreatedDate.Year;
            UserId = myDigitalContent.UserId;
            DigitalContentId = myDigitalContent.DigitalContentId;
            Status = myDigitalContent.Status;
            DigitalContentType = myDigitalContent.DigitalContentType;
            Version = myDigitalContent.Version;
            ReviewStatus = myDigitalContent.ReviewStatus;
            ProgressMeasure = myDigitalContent.ProgressMeasure;
            DisenrollUtc = myDigitalContent.DisenrollUtc;
            ReadDate = myDigitalContent.ReadDate;
            ReminderSentDate = myDigitalContent.ReminderSentDate;
            StartDate = myDigitalContent.StartDate;
            EndDate = myDigitalContent.EndDate;
            CompletedDate = myDigitalContent.CompletedDate;
            RecordUri = $"learner:{myDigitalContent.CreatedDate.Year}:{myDigitalContent.DigitalContentType.ToString().ToLower()}:{myDigitalContent.DigitalContentId}:user:{myDigitalContent.UserId}";
        }

        public int Year { get; set; }

        public Guid UserId { get; set; }

        public Guid DigitalContentId { get; set; }

        public MyDigitalContentStatus Status { get; set; }

        public DigitalContentType DigitalContentType { get; set; }

        public string Version { get; set; }

        public string ReviewStatus { get; set; }

        public double? ProgressMeasure { get; set; }

        public DateTime? DisenrollUtc { get; set; }

        public DateTime? ReadDate { get; set; }

        public DateTime? ReminderSentDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public string RecordUri { get; set; }

        public string RecordType { get; set; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.learner.digitalcontent.{Status.ToString().ToLower()}";
        }
    }
}
