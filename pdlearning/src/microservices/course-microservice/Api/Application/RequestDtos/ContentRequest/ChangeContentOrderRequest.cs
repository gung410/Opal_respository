using System;
using Microservice.Course.Application.Models;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeContentOrderRequest
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public MovementDirection Direction { get; set; }

        public MovementContentType Type { get; set; }
    }
}
