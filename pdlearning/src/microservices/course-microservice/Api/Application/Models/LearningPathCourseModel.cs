using System;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Models
{
    public class LearningPathCourseModel
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public int? Order { get; set; }

        public static LearningPathCourseModel Create(LearningPathCourse entity)
        {
            return new LearningPathCourseModel
            {
                Id = entity.Id,
                CourseId = entity.CourseId,
                Order = entity.Order
            };
        }
    }
}
