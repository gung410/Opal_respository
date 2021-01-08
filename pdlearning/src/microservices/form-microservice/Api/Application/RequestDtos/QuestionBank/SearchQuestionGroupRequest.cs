using Microservice.Form.Application.Queries;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Form.Application.RequestDtos
{
    public class SearchQuestionGroupRequest
    {
        public string Name { get; set; }

        public bool IsFilterByUsing { get; set; } = false;

        public PagedResultRequestDto PagedInfo { get; set; }

        public SearchQuestionGroupQuery ToQuery()
        {
            return new SearchQuestionGroupQuery
            {
                Name = Name,
                PagedInfo = PagedInfo,
                IsFilterByUsing = IsFilterByUsing
            };
        }
    }
}
