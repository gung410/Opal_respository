using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Form.Application.RequestDtos
{
    public class SearchFormQuestionsRequestDto
    {
        public Guid FormId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
