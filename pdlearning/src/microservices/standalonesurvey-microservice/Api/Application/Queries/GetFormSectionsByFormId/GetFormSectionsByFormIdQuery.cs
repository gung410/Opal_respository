using System;
using System.Collections.Generic;
using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetFormSectionsByFormIdQuery : BaseStandaloneSurveyQuery<List<SurveySectionModel>>
    {
        public Guid FormId { get; set; }
    }
}
