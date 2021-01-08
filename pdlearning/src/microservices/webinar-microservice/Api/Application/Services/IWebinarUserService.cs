using System.Threading.Tasks;
using Microservice.Webinar.Application.RequestDtos;

namespace Microservice.Webinar.Application.Services
{
    public interface IWebinarUserService
    {
        Task SaveUser(SaveWebinarUserRequestDto request);
    }
}
