using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class SearchLearningPathRequest : PagedResultRequestDto
    {
        public string SearchText { get; set; }
    }
}
