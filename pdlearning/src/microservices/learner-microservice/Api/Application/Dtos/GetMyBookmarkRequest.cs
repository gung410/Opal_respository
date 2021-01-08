using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Learner.Application.Dtos
{
    public class GetMyBookmarkRequest : PagedResultRequestDto
    {
        public BookmarkType ItemType { get; set; }
    }
}
