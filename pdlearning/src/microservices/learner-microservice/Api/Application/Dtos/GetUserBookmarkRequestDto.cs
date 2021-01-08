using System.Collections.Generic;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Learner.Application.Dtos
{
    public class GetUserBookmarkRequestDto : PagedResultRequestDto
    {
        public List<BookmarkType> BookmarkTypeFilter { get; set; }
    }
}
