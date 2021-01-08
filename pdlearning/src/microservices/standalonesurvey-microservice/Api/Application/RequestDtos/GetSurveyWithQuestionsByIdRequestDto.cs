using System;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class GetSurveyWithQuestionsByIdRequestDto
    {
        public Guid FormId { get; set; }

        public bool OnlyPublished { get; set; }
    }
}
