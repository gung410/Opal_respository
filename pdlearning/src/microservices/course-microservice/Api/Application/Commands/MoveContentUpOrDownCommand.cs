using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class MoveContentUpOrDownCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public MovementDirection Direction { get; set; }

        public MovementContentType Type { get; set; }
    }
}
