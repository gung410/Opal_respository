using System;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Domain.Entities
{
    public class CourseInternalValue
    {
        public int Id { get; set; }

        public Guid CourseId { get; set; }

        public CourseInternalValueType Type { get; set; }

        public string Value { get; set; }

        public virtual CourseEntity Course { get; set; }

        public static CourseInternalValue Create(Guid courseId, CourseInternalValueType courseInternalValueType, Guid value)
        {
            return new CourseInternalValue
            {
                CourseId = courseId,
                Type = courseInternalValueType,
                Value = value.ToString()
            };
        }

        public static CourseInternalValue Create(Guid courseId, CourseInternalValueType courseInternalValueType, int value)
        {
            return new CourseInternalValue
            {
                CourseId = courseId,
                Type = courseInternalValueType,
                Value = value.ToString()
            };
        }

        public static CourseInternalValue Create(Guid courseId, CourseInternalValueType courseInternalValueType, string value)
        {
            return new CourseInternalValue
            {
                CourseId = courseId,
                Type = courseInternalValueType,
                Value = value
            };
        }
    }
}
