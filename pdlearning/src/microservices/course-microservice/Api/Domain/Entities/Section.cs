using System;
using Microservice.Course.Domain.Entities.Abstractions;
using Microservice.Course.Domain.Interfaces;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Domain.Entities
{
    public class Section : BaseOrderableContent, ISoftDelete, IOrderable
    {
        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid ChangedBy { get; set; }

        public int? CreditsAward { get; set; }

        public string ExternalID { get; set; }

        public Section CloneForClassRun(Guid classRunId, Guid createdBy)
        {
            return new Section
            {
                Id = Guid.NewGuid(),
                Description = Description,
                CourseId = CourseId,
                ClassRunId = classRunId,
                CreatedDate = Clock.Now,
                CreatedBy = createdBy,
                Title = Title,
                Order = Order,
                CreditsAward = CreditsAward
            };
        }

        public Section CloneForCourse(Guid courseId, Guid createdBy, int fromOrder = 0)
        {
            return new Section
            {
                Id = Guid.NewGuid(),
                Description = Description,
                CourseId = courseId,
                ClassRunId = ClassRunId,
                CreatedDate = Clock.Now,
                CreatedBy = createdBy,
                Title = Title,
                Order = Order + fromOrder ?? fromOrder,
                CreditsAward = CreditsAward
            };
        }

        public override Guid ForTargetId()
        {
            return ClassRunId ?? CourseId;
        }
    }
}
