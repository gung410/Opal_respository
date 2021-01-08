using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Course.Application.Enums;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class ECertificateEvent : BaseThunderEvent, IMQMessage
    {
        public ECertificateEvent()
        {
        }

        public ECertificateEvent(Registration registration, string fileBase64String, RecordType recordType, Guid correlationId)
        {
            RecordUri = $"CAM:{registration.CreatedDate.Year}:{recordType}:{registration.CourseId}:user:{registration.UserId}";
            RecordType = recordType;
            UserId = registration.UserId;
            Year = registration.CreatedDate.Year;
            CourseId = registration.CourseId;
            CorrelationId = correlationId;
            File = new ECertificatePayload()
            {
                Mime = "application/pdf",
                Data = fileBase64String,
                Name = string.Empty
            };
        }

        public string RecordUri { get; set; }

        public RecordType RecordType { get; set; }

        public string FileBase64String { get; set; }

        public Guid UserId { get; set; }

        public int Year { get; set; }

        public Guid CourseId { get; set; }

        public Guid CorrelationId { get; set; }

        public ECertificatePayload File { get; set; }
    }
}
