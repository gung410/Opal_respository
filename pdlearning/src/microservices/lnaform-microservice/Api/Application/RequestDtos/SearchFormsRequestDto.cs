using System.Collections.Generic;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.LnaForm.Application.RequestDtos
{
    public class SearchFormsRequestDto
    {
        public PagedResultRequestDto PagedInfo { get; set; }

        public string SearchFormTitle { get; set; }

        public IEnumerable<FormStatus> FilterByStatus { get; set; }

        public bool IncludeFormForImportToCourse { get; set; }
    }
}
