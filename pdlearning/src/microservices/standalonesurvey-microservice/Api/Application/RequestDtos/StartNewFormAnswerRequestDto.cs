using System;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class SaveSurveyAnswerRequestDto : HasSubModuleInfoBase
    {
        public Guid FormId { get; set; }

        public Guid? ResourceId { get; set; }
    }
}
