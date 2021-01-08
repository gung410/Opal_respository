using System.Collections.Generic;
using Microservice.Form.Domain.ValueObjects.Form;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Form.Application.RequestDtos
{
    public class SearchFormsRequestDto
    {
        public PagedResultRequestDto PagedInfo { get; set; }

        public string SearchFormTitle { get; set; }

        public IEnumerable<FormStatus> FilterByStatus { get; set; }

        public bool IncludeFormForImportToCourse { get; set; }

        public FormType? FilterByType { get; set; }

        public IEnumerable<FormSurveyType> FilterBySurveyTypes { get; set; }

        public IEnumerable<FormSurveyType> ExcludeBySurveyTypes { get; set; }

        public bool? IsSurveyTemplate { get; set; }
    }
}
