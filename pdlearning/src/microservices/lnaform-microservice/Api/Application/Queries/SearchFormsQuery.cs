using System;
using System.Collections.Generic;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class SearchFormsQuery : BaseThunderQuery<PagedResultDto<FormModel>>
    {
        public Guid UserId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }

        public string SearchFormTitle { get; set; }

        public IEnumerable<FormStatus> FilterByStatus { get; set; }

        public bool IncludeFormForImportToCourse { get; set; }
    }
}
