using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SaveBlockoutDateCommand : BaseThunderCommand
    {
        public SaveBlockoutDateCommand()
        {
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public BlockoutDateStatus Status { get; set; }

        public string Description { get; set; }

        public int StartDay { get; set; }

        public int StartMonth { get; set; }

        public int EndDay { get; set; }

        public int EndMonth { get; set; }

        public Guid PlanningCycleId { get; set; }

        public IEnumerable<string> ServiceSchemes { get; set; } = new List<string>();

        public bool IsCreate { get; set; }
    }
}
