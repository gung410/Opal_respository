using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SaveCoursePlanningCycleCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public int YearCycle { get; set; }

        public string Title { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Description { get; set; }

        public bool IsCreate { get; set; }
    }
}
