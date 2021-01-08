using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Commands
{
    public class ArchiveFormCommand : BaseThunderCommand
    {
        public Guid FormId { get; set; }

        public Guid? ArchiveBy { get; set; }
    }
}
