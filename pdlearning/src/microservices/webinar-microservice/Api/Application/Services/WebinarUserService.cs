using System.Threading.Tasks;
using Microservice.Webinar.Application.RequestDtos;
using Microservice.Webinar.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Webinar.Application.Services
{
    public class WebinarUserService : IWebinarUserService
    {
        private readonly IRepository<WebinarUser> _userRepository;

        public WebinarUserService(IRepository<WebinarUser> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task SaveUser(SaveWebinarUserRequestDto request)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.OriginalUserId == request.Identity.Id && u.Id == request.Identity.ExtId);
            if (user == null)
            {
                user = new WebinarUser()
                {
                    Id = request.Identity.ExtId,
                    OriginalUserId = request.Identity.Id,
                    AvatarUrl = request.AvatarUrl,
                    DepartmentId = request.DepartmentId,
                    Email = request.EmailAddress,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };

                await _userRepository.InsertAsync(user);
            }
            else
            {
                user.AvatarUrl = request.AvatarUrl;
                user.DepartmentId = request.DepartmentId;
                user.Email = request.EmailAddress;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;

                await _userRepository.UpdateAsync(user);
            }
        }
    }
}
