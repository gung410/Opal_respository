using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class SearchCoursesPlanningCyclesRequest : PagedResultRequestDto
    {
        public string SearchText { get; set; }
    }
}
