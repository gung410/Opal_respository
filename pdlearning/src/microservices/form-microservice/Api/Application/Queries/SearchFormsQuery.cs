using System;
using System.Collections.Generic;
using Microservice.Form.Application.Models;
using Microservice.Form.Domain.ValueObjects.Form;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class SearchFormsQuery : BaseThunderQuery<PagedResultDto<FormModel>>
    {
        public Guid UserId { get; set; }

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
