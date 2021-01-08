using System;
using Microservice.StandaloneSurvey.Application.Services;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class SaveSurveySectionCommand : BaseStandaloneSurveyCommand
    {
        public CreateSurveySectionRequestDto CreationRequest { get; set; }

        public UpdateFormSectionRequestDto UpdateRequest { get; set; }

        public Guid UserId { get; set; }

        public bool IsCreation { get; set; }
    }
}
