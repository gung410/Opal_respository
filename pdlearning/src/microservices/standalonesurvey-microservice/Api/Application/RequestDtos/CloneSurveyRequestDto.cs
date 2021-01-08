using System;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class CloneSurveyRequestDto : HasSubModuleInfoBase
    {
        public Guid FormId { get; set; }

        public string NewTitle { get; set; }
    }
}
