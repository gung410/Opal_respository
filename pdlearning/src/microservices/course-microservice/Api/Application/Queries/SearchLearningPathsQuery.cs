using Microservice.Course.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class SearchLearningPathsQuery : BaseThunderQuery<PagedResultDto<LearningPathModel>>
    {
        public string SearchText { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
