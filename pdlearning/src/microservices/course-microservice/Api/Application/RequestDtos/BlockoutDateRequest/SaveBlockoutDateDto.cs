using System;
using System.Collections.Generic;
using Microservice.Course.Application.Commands;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveBlockoutDateDto
    {
        public Guid? Id { get; set; }

        public string Title { get; set; }

        public BlockoutDateStatus Status { get; set; } = BlockoutDateStatus.Draft;

        public string Description { get; set; }

        public int StartDay { get; set; }

        public int StartMonth { get; set; }

        public int EndDay { get; set; }

        public int EndMonth { get; set; }

        public Guid PlanningCycleId { get; set; }

        public IEnumerable<string> ServiceSchemes { get; set; } = new List<string>();

        public SaveBlockoutDateCommand ToSaveBlockoutDateCommand()
        {
            return new SaveBlockoutDateCommand()
            {
                Id = Id ?? Guid.NewGuid(),
                Title = Title,
                Description = Description,
                IsCreate = !Id.HasValue,
                StartDay = StartDay,
                StartMonth = StartMonth,
                EndDay = EndDay,
                EndMonth = EndMonth,
                PlanningCycleId = PlanningCycleId,
                ServiceSchemes = ServiceSchemes,
                Status = Status
            };
        }
    }
}
