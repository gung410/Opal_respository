using System.Collections.Generic;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class SearchSurveysRequestDto : HasSubModuleInfoBase
    {
        public PagedResultRequestDto PagedInfo { get; set; }

        public string SearchFormTitle { get; set; }

        public IEnumerable<SurveyStatus> FilterByStatus { get; set; }

        public bool IncludeFormForImportToCourse { get; set; }

        // CSL only
        public bool OnlyCslSurveysForManagement { get; set; }
    }
}
