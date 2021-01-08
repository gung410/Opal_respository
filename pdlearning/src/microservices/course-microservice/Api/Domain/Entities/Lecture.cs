using System;
using Microservice.Course.Domain.Entities.Abstractions;
using Microservice.Course.Domain.Interfaces;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Domain.Entities
{
    public class Lecture : BaseOrderableContent, ISoftDelete, IOrderable, IContent
    {
        public Guid? SectionId { get; set; }

        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public string LectureName { get; set; }

        public string LectureIcon { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid ChangedBy { get; set; }

        public string ExternalID { get; set; }

        public Lecture CloneForClassRun(Guid classRunId, Guid createdBy, Guid? newSectionId = null)
        {
            return new Lecture
            {
                Id = Guid.NewGuid(),
                Description = Description,
                CreatedDate = Clock.Now,
                Type = Type,
                CreatedBy = createdBy,
                CourseId = CourseId,
                ClassRunId = classRunId,
                LectureName = LectureName,
                SectionId = newSectionId,
                Order = Order,
                LectureIcon = LectureIcon
            };
        }

        public Lecture CloneForCourse(Guid courseId, Guid createdBy, Guid? newSectionId = null, int fromOrderNumber = 0)
        {
            return new Lecture
            {
                Id = Guid.NewGuid(),
                Description = Description,
                CreatedDate = Clock.Now,
                Type = Type,
                CreatedBy = createdBy,
                CourseId = courseId,
                ClassRunId = ClassRunId,
                LectureName = LectureName,
                SectionId = newSectionId,
                Order = newSectionId != null ? Order : Order + fromOrderNumber ?? fromOrderNumber,
                LectureIcon = LectureIcon
            };
        }

        public override Guid ForTargetId()
        {
            return ClassRunId ?? CourseId;
        }
    }
}
