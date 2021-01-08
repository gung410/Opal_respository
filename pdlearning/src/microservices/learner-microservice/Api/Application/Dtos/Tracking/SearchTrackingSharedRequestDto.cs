using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Learner.Application.Dtos
{
    public class SearchTrackingSharedRequestDto : PagedResultRequestDto
    {
        public Guid UserId { get; set; }
    }
}
