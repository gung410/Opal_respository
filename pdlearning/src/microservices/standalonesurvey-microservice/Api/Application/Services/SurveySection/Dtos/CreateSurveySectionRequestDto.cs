using System;
using Microservice.StandaloneSurvey.Application.Commands;
using Microservice.StandaloneSurvey.Application.RequestDtos;

namespace Microservice.StandaloneSurvey.Application.Services
{
    public class CreateSurveySectionRequestDto : HasSubModuleInfoBase
    {
        public Guid? Id { get; set; }

        public Guid SurveyId { get; set; }

        public string MainDescription { get; set; }

        public string AdditionalDescription { get; set; }

        public int Priority { get; set; }

        public Guid? NextQuestionId { get; set; }

        public bool IsDeleted { get; set; }

        public SaveSurveySectionsCommand BuildSurveySectionCommand()
        {
            return new SaveSurveySectionsCommand
            {
                Id = Id,
                SurveyId = SurveyId,
                MainDescription = MainDescription,
                AdditionalDescription = AdditionalDescription,
                Priority = Priority,
                NextQuestionId = NextQuestionId,
                IsDeleted = IsDeleted
            };
        }
    }
}
