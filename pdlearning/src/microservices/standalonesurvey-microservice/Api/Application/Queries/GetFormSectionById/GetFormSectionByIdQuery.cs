using System;
using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetFormSectionByIdQuery : BaseStandaloneSurveyQuery<SurveySectionModel>
    {
        public Guid Id { get; set; }
    }
}
