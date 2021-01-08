using System;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Domain.Entities
{
    public class ClassRunInternalValue
    {
        public int Id { get; set; }

        public Guid ClassRunId { get; set; }

        public ClassRunInternalValueType Type { get; set; }

        public string Value { get; set; }

        public virtual ClassRun ClassRun { get; set; }

        public static ClassRunInternalValue Create(Guid classRunId, ClassRunInternalValueType classRunInternalValueType, Guid value)
        {
            return new ClassRunInternalValue
            {
                ClassRunId = classRunId,
                Type = classRunInternalValueType,
                Value = value.ToString()
            };
        }

        public static ClassRunInternalValue Create(Guid classRunId, ClassRunInternalValueType classRunInternalValueType, string value)
        {
            return new ClassRunInternalValue
            {
                ClassRunId = classRunId,
                Type = classRunInternalValueType,
                Value = value
            };
        }
    }
}
