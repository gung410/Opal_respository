using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Models
{
    public class ContentItem
    {
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }

        public Guid CourseId { get; set; }

        public Guid? ClassrunId { get; set; }

        public Guid CreatedBy { get; set; }

        public string Title { get; set; }

        public string Icon { get; set; }

        public ContentType Type { get; set; }

        public List<ContentItem> Items { get; set; }

        public int? Order { get; set; }

        public object AdditionalInfo { get; set; }

        public static ContentItem CreateForSection(Section section, IEnumerable<Lecture> sectionLectures, Dictionary<Guid, LectureContent> lectureToContentMap, bool includeAdditionalInfo = true)
        {
            return new ContentItem()
            {
                Id = section.Id,
                CourseId = section.CourseId,
                ClassrunId = section.ClassRunId,
                CreatedBy = section.CreatedBy,
                Title = section.Title,
                Type = ContentType.Section,
                Order = section.Order,
                AdditionalInfo = includeAdditionalInfo
                ? SectionModel.Create(section)
                : null,
                Items = sectionLectures?.Select(p => CreateForLecture(p, lectureToContentMap.ContainsKey(p.Id) ? lectureToContentMap[p.Id] : null)).ToList()
            };
        }

        public static ContentItem CreateForLecture(Lecture lecture, LectureContent content = null, bool includeAdditionalInfo = true)
        {
            return new ContentItem()
            {
                Id = lecture.Id,
                ParentId = lecture.SectionId,
                CourseId = lecture.CourseId,
                ClassrunId = lecture.ClassRunId,
                CreatedBy = lecture.CreatedBy,
                Title = lecture.LectureName,
                Icon = lecture.LectureIcon,
                Type = ContentType.Lecture,
                Order = lecture.Order,
                AdditionalInfo = content != null && includeAdditionalInfo
                ? LectureModel.Create(lecture, content)
                : null
            };
        }

        public static ContentItem CreateForAssignment(Assignment assignment, bool includeAdditionalInfo = true)
        {
            return new ContentItem()
            {
                Id = assignment.Id,
                CourseId = assignment.CourseId,
                ClassrunId = assignment.ClassRunId,
                CreatedBy = assignment.CreatedBy,
                Title = assignment.Title,
                Type = ContentType.Assignment,
                AdditionalInfo = includeAdditionalInfo
                ? new AssignmentModel(assignment)
                : null
            };
        }
    }
}
