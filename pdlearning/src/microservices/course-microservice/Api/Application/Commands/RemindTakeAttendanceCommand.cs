using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class RemindTakeAttendanceCommand : BaseThunderCommand
    {
        public DateTime? ForSessionEndTimeAfter { get; set; }

        public DateTime? ForSessionEndTimeBefore { get; set; }
    }
}
