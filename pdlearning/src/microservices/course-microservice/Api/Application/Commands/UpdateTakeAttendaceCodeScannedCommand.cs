using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class UpdateTakeAttendaceCodeScannedCommand : BaseThunderCommand
    {
        public Guid SessionId { get; set; }

        public string SessionCode { get; set; }
    }
}
