using System;
using System.Collections.Generic;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class SearchFormsQuery : BaseStandaloneSurveyQuery<PagedResultDto<StandaloneSurveyModel>>
    {
        public Guid UserId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }

        public string SearchFormTitle { get; set; }

        public IEnumerable<SurveyStatus> FilterByStatus { get; set; }

        public bool IncludeFormForImportToCourse { get; set; }

        public bool IsOnlyCslSurveysForManagement { get; set; }
    }
}
