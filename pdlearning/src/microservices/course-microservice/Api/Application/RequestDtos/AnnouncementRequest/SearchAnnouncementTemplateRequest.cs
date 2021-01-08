using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class SearchAnnouncementTemplateRequest : PagedResultRequestDto
    {
        public string SearchText { get; set; }
    }
}
