using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Badge.Application.RequestDtos
{
    public class SearchTopBadgeUserRequest : PagedResultRequestDto
    {
        public Guid BadgeId { get; set; }

        public string SearchText { get; set; }
    }
}
