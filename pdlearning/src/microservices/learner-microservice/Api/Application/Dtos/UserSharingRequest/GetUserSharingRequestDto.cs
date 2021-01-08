using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Learner.Application.Dtos
{
    public class GetUserSharingRequestDto : PagedResultRequestDto
    {
        public string SearchText { get; set; }

        public SharingType ItemType { get; set; }
    }
}
