using System;
using System.Text;
using Microservice.Course.Application.Models;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class CreateOrUpdateLectureCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public bool IsCreateNew { get; set; }

        public string Description { get; set; }

        public string LectureName { get; set; }

        public string LectureIcon { get; set; }

        public int? Order { get; set; }

        public string MimeType { get; set; }

        public Guid? ResourceId { get; set; }

        public LectureContentType Type { get; set; }

        public string Base64Value { get; set; }

        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public Guid? SectionId { get; set; }

        public QuizConfigModel QuizConfig { get; set; }

        public DigitalContentConfigModel DigitalContentConfig { get; set; }

        public string GetDecodedValue()
        {
            return Base64Value == null ? string.Empty : Encoding.UTF8.GetString(Convert.FromBase64String(Base64Value));
        }
    }
}
