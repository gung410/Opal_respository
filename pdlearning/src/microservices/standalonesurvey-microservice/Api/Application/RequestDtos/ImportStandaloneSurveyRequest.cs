using System.Collections.Generic;
using Microservice.StandaloneSurvey.Application.Models;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class ImportStandaloneSurveyRequest : HasSubModuleInfoBase
    {
       public List<ImportStandaloneSurveyModel> FormWithQuestionsSections { get; set; }
    }
}
