using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
{
    public class DeleteFormParticipantCommand : BaseThunderCommand
    {
        public Guid[] Ids { get; set; }

        public Guid FormId { get; set; }
    }
}
