using System;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveLearningPathCourseDto
    {
        public Guid? Id { get; set; }

        public Guid CourseId { get; set; }

        public int? Order { get; set; }

        public LearningPathCourse Create(Guid learningPathId, Guid courseId, int order)
        {
            return new LearningPathCourse
            {
                Id = Guid.NewGuid(),
                LearningPathId = learningPathId,
                CreatedDate = Clock.Now,
                CourseId = courseId,
                Order = order
            };
        }

        public LearningPathCourse Update(LearningPathCourse currentLearningPathCourse)
        {
            if (currentLearningPathCourse.Order != Order)
            {
                currentLearningPathCourse.Order = Order;
                currentLearningPathCourse.ChangedDate = Clock.Now;
            }

            return currentLearningPathCourse;
        }
    }
}
