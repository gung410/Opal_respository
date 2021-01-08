using System;
using Microservice.Course.Application.Commands;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveCoursePlanningCycleDto
    {
        public Guid? Id { get; set; }

        public int YearCycle { get; set; }

        public string Title { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Description { get; set; }

        public SaveCoursePlanningCycleCommand ToCommand()
        {
            return new SaveCoursePlanningCycleCommand()
            {
                Id = Id ?? Guid.NewGuid(),
                IsCreate = !Id.HasValue,
                YearCycle = YearCycle,
                Title = Title,
                StartDate = StartDate,
                EndDate = EndDate,
                Description = Description,
            };
        }
    }
}
