using Conexus.Opal.AccessControl.RequestDtos;

namespace Microservice.Webinar.Application.RequestDtos
{
    public class SaveWebinarUserRequestDto : SaveUserRequestDto
    {
        public string AvatarUrl { get; set; }
    }
}
