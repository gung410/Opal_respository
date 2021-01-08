using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class UpdateMyCourseStatusCommand : BaseThunderCommand
    {
        public Guid CourseId { get; set; }

        public MyCourseStatus Status { get; set; }
    }
}
