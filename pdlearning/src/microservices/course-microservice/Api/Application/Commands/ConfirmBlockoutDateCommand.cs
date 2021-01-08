using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ConfirmBlockoutDateCommand : BaseThunderCommand
    {
        public Guid CoursePlanningCycleId { get; set; }
    }
}
