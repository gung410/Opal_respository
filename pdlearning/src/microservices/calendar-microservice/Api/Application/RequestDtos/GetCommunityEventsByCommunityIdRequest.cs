using System;
using Microservice.Calendar.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Calendar.Application.RequestDtos
{
    public class GetCommunityEventsByCommunityIdRequest : PagedResultRequestDto
    {
        [BindProperty(Name = "communityId")]
        public Guid CommunityId { get; set; }

        [BindProperty(Name = "eventType")]
        public CalendarEventSource CalendarEventSource { get; set; }
    }
}
