using System;
using System.Collections.Generic;
using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Commands
{
    public class ImportFormCommand : BaseThunderCommand
    {
        public int DepartmentId { get; set; }

        public Guid UserId { get; set; }

        public List<ImportFormModel> FormWithSectionsQuestions { get; set; }
    }
}
