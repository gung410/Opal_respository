using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.LnaForm.Application.RequestDtos
{
    public class ArchiveFormRequest
    {
        public Guid ObjectId { get; set; }

        public Guid? ArchiveByUserId { get; set; }
    }
}
