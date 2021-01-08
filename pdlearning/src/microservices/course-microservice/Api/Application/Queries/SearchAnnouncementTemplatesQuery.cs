using Microservice.Course.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class SearchAnnouncementTemplatesQuery : BaseThunderQuery<PagedResultDto<AnnouncementTemplateModel>>
    {
        public string SearchText { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
