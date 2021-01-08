using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
{
    public class ArchiveFormCommand : BaseThunderCommand
    {
        public Guid FormId { get; set; }

        public Guid? ArchiveBy { get; set; }
    }
}
