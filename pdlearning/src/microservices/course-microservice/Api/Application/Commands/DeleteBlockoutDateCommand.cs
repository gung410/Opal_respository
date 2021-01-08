using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class DeleteBlockoutDateCommand : BaseThunderCommand
    {
        public Guid BlockoutDateId { get; set; }
    }
}
