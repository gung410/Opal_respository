using System;
using Microservice.Learner.Domain.Entities;
using Thunder.Platform.Core.Timing;

namespace Microservice.Learner.Application.Dtos
{
    public class SaveLearnerLearningPathCourseRequestDto
    {
        public Guid? Id { get; set; }

        public Guid CourseId { get; set; }

        public int? Order { get; set; }

        public LearnerLearningPathCourse CreateNewLearningPathCourse(Guid learningPathId, Guid courseId, int order)
        {
            return new LearnerLearningPathCourse
            {
                Id = Guid.NewGuid(),
                LearningPathId = learningPathId,
                CreatedDate = Clock.Now,
                CourseId = courseId,
                Order = order
            };
        }

        public LearnerLearningPathCourse UpdateExistedLearningPathCourse(LearnerLearningPathCourse currentLearningPathCourse)
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
