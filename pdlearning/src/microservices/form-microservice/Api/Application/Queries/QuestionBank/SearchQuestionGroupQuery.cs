using Microservice.Form.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class SearchQuestionGroupQuery : BaseThunderQuery<PagedResultDto<QuestionGroupModel>>
    {
        public string Name { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }

        public bool IsFilterByUsing { get; set; } = false;
    }
}
