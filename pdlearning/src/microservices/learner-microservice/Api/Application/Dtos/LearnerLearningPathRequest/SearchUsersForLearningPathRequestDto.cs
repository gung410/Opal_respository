using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Learner.Application.Dtos
{
    public class SearchUsersForLearningPathRequestDto : PagedResultRequestDto
    {
        public string SearchText { get; set; }
    }
}
