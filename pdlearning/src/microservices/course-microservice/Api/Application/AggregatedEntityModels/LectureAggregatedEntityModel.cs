using System;
using Microservice.Course.Application.AggregatedEntityModels.Abstractions;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.AggregatedEntityModels
{
    public class LectureAggregatedEntityModel : BaseAggregatedEntityModel, IAggregatedContentEntityModel
    {
        public Lecture Lecture { get; init; }

        public LectureContent LectureContent { get; init; }

        public CourseUser Owner { get; init; }

        public Guid Id => Lecture.Id;

        public Guid ForTargetId => Lecture.ForTargetId();

        public Guid OwnerId => Lecture.CreatedBy;

        public Guid CourseId => Lecture.CourseId;

        public Guid? ClassRunId => Lecture.ClassRunId;

        public string Title => Lecture.LectureName;

        public static LectureAggregatedEntityModel Create(Lecture lecture, LectureContent lectureContent, CourseUser owner = null)
        {
            return new LectureAggregatedEntityModel
            {
                Lecture = lecture,
                LectureContent = lectureContent,
                Owner = owner
            };
        }

        public string CombinedRichText()
        {
            return LectureContent.Value;
        }
    }
}
