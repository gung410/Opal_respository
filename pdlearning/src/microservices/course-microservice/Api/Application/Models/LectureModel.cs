using System;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Models
{
    public class LectureModel
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public Guid? SectionId { get; set; }

        public Guid? ClassRunId { get; set; }

        public string Title { get; set; }

        public string Icon { get; set; }

        public string Description { get; set; }

        public LectureContentType Type { get; set; }

        public string Value { get; set; }

        public Guid ResourceId { get; set; }

        public LectureQuizConfig QuizConfig { get; set; }

        public LectureDigitalContentConfig DigitalContentConfig { get; set; }

        /// <summary>
        /// Currently, we intend to support these mime types:
        /// text/plain, image/jpeg, image/png, text/html, application/vnd.opal2.popup,
        /// application/vnd.opal2.html5, application/vnd.opal2.iframe, video/youtube, video/mp4
        /// application/pdf, text/url, application/vnd.opal2.scorm, application/vnd.omnimetrio.quiz..
        /// This list is gotten from OPAL legacy.
        /// </summary>
        public string MimeType { get; set; }

        public int? Order { get; set; }

        public static LectureModel Create(Lecture lecture, LectureContent lectureContent)
        {
            return new LectureModel
            {
                Id = lecture.Id,
                CourseId = lecture.CourseId,
                SectionId = lecture.SectionId,
                Description = lecture.Description,
                Title = lecture.LectureName,
                Type = lectureContent.Type,
                Value = lectureContent.Value,
                ResourceId = lectureContent.ResourceId ?? Guid.Empty,
                MimeType = lectureContent.MimeType,
                Icon = lecture.LectureIcon,
                Order = lecture.Order,
                ClassRunId = lecture.ClassRunId,
                QuizConfig = lectureContent.QuizConfig,
                DigitalContentConfig = lectureContent.DigitalContentConfig
            };
        }
    }
}
