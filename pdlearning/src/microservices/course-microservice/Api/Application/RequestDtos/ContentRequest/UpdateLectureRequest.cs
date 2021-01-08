using System;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.Models;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveLectureRequest
    {
        public Guid? Id { get; set; }

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

        public CreateOrUpdateLectureCommand ToCommand()
        {
            var id = Id ?? Guid.NewGuid();
            return new CreateOrUpdateLectureCommand
            {
                Id = id,
                Description = Description,
                LectureName = LectureName,
                LectureIcon = LectureIcon,
                Order = Order,
                MimeType = MimeType,
                ResourceId = ResourceId,
                Type = Type,
                Base64Value = Base64Value,
                CourseId = CourseId,
                ClassRunId = ClassRunId,
                SectionId = SectionId,
                IsCreateNew = Id == null,
                QuizConfig = QuizConfig,
                DigitalContentConfig = DigitalContentConfig
            };
        }
    }
}
