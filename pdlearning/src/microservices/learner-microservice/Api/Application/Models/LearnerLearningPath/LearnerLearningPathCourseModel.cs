using System;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Application.Models
{
    public class LearnerLearningPathCourseModel
    {
        public LearnerLearningPathCourseModel(LearnerLearningPathCourse entity)
        {
            Id = entity.Id;
            CourseId = entity.CourseId;
            Order = entity.Order;
            LearningPathId = entity.LearningPathId;
        }

        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public int? Order { get; set; }

        public Guid LearningPathId { get; set; }
    }
}
