using System;
using System.Collections.Generic;
using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class ImportFormCommand : BaseStandaloneSurveyCommand
    {
        public int DepartmentId { get; set; }

        public Guid UserId { get; set; }

        public List<ImportStandaloneSurveyModel> FormWithSectionsQuestions { get; set; }
    }
}
